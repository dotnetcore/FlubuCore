using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers.WebApp
{
    [Route("[controller]")]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    public class LoginController : Controller
    {
        private readonly IWebApiClient _webApiClient;

        public LoginController(IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
        }

        public ActionResult Index()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromForm]LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            //// todo move to web api client initialization.
            if (string.IsNullOrEmpty(_webApiClient.WebApiBaseUrl))
            {
                _webApiClient.WebApiBaseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/";
            }

            Response<GetTokenResponse> response = await _webApiClient.ExecuteAsync(async x =>
                await x.GetTokenAsync(new GetTokenRequest()
                {
                    Username = model.Username,
                    Password = model.Password
                }), HttpStatusCode.BadRequest, HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);

            if (response.Error != null)
            {
                if (response.Error.ErrorCode == GetTokenRequest.WrongUsernamePassword)
                {
                    ModelState.AddModelError("WrongUserNameOrPassword", "Wrong username or password.");
                    return View("Index", model);
                }

                if (response.Error.StatusCode == HttpStatusCode.Unauthorized || response.Error.StatusCode == HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError("Forbiden", "Forbiden access. See api logs for details.");
                    return View("Index", model);
                }
            }

            if (response.Data == null || string.IsNullOrEmpty(response.Data.Token))
            {
                ModelState.AddModelError("Technical", "Technical error. See api logs for details.");
                return View("Index", model);
            }

            var claims = new List<Claim>()
            {
            };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userPrincipal));

            return RedirectToAction("Index", "UpdateCenter");
        }
    }
}