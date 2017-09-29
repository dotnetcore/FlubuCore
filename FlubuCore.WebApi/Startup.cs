using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Services;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Infrastructure;
using FlubuCore.WebApi.Repository;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlubuCore.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        private string _secretKey;

        private SymmetricSecurityKey _signingKey;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
                {
                    options.ModelValidatorProviders.Clear();
                })
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services
                .AddCoreComponents()
                .AddCommandComponents()
                .AddScriptAnalyser()
                .AddTasks();

            services.AddScoped<ApiExceptionFilter>();
            services.AddScoped<ValidateRequestModelAttribute>();
	        services.AddScoped<RestrictApiAccessFilter>();
	        services.AddScoped<IHashService, HashService>();
	        services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISecurityRepository, SecurityRepository>();
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtOptions));
            _secretKey = jwtAppSettingOptions["secretKey"];
            double validFor = double.Parse(jwtAppSettingOptions[(nameof(JwtOptions.ValidFor))]);
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
			
            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
                options.ValidFor = TimeSpan.FromMinutes(validFor);
            });

	        services.Configure<WebApiSettings>(settings => Configuration.GetSection(nameof(WebApiSettings)).Bind(settings));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/Flubu-{Date}.txt");

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseMvc();
        }
    }
}
