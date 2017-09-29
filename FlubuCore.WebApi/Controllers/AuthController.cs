using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FlubuCore.Services;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
	    private readonly IHashService _hashService;
	    private readonly IUserRepository _userRepository;
        private readonly ISecurityRepository _securityRepository;

        public AuthController(IOptions<JwtOptions> jwtOptions, ILoggerFactory loggerFactory, IUserRepository userRepository, ISecurityRepository securityRepository, IHashService hashService)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _securityRepository = securityRepository;
	        _hashService = hashService;
	        _userRepository = userRepository;
            _logger = loggerFactory.CreateLogger<AuthController>();
			_serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] GetTokenRequest applicationUser)
        {
            var securityTask = _securityRepository.GetSecurityAsync();
            var security = await securityTask;
            
            if (security.ApiAccessDisabled)
            {
                _logger.LogWarning("Api access is denied becasuse to many failed get token attempts. To enable access open manually Security.json file and set property ApiAccessDisabled to false. ");
                 throw new HttpError(HttpStatusCode.Forbidden);
            }

            var identityTask = GetClaimsIdentity(applicationUser, security);
            var identity = await identityTask;


            if (identity == null)
            {
                _logger.LogInformation($"Invalid username ({applicationUser.Username}) or password ({applicationUser.Password})");
                _securityRepository.IncreaseFailedGetTokenAttempts(security);
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Username),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            var response2 = new GetTokenResponse
            {
                Token = encodedJwt,
                ExpiresIn = (int) _jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response2, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(JwtOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

	   private async Task<ClaimsIdentity> GetClaimsIdentity(GetTokenRequest user, Security security)
	    {
		    var users = await _userRepository.ListUsersAsync();
		    var usr = users.FirstOrDefault(x => x.Username == user.Username);
		    if (usr == null)
		    {
                _securityRepository.IncreaseFailedGetTokenAttempts(security);
                throw new HttpError(HttpStatusCode.BadRequest, "WrongUsernameOrPassword");
		    }

		    var passwordValid = _hashService.Validate(user.Password, usr.Password);
		    if (!passwordValid)
		    {
                _securityRepository.IncreaseFailedGetTokenAttempts(security);
                throw new HttpError(HttpStatusCode.BadRequest, "WrongUsernameOrPassword");
		    }

		    return new ClaimsIdentity(new GenericIdentity(user.Username, "Token"),
			    new Claim[] { });
	    }
    }
}
