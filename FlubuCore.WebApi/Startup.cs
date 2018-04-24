using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Infrastructure;
using FlubuCore.WebApi.Repository;
using FlubuCore.WebApi.Services;
using FluentValidation.AspNetCore;
using LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Logger = Serilog.Core.Logger;

namespace FlubuCore.WebApi
{
    public class Startup
    {
        private string _secretKey;

        private SymmetricSecurityKey _signingKey;

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
                .AddCoreComponentsForWebApi()
                .AddCommandComponentsForWebApi()
                .AddScriptAnalyserForWebApi()
                .AddTasksForWebApi();

            var connectionStrings = Configuration.GetSection("FlubuConnectionStrings");
            var liteDbConnectionString = connectionStrings["LiteDbConnectionString"];

            var db = new LiteRepository(liteDbConnectionString);
            ILiteRepositoryFactory liteRepositoryFactory = new LiteRepositoryFactory();
            services.AddSingleton(liteRepositoryFactory);
            services.AddSingleton<IRepositoryFactory>(new RepositoryFactory(liteRepositoryFactory, new TimeProvider()));
            services.AddSingleton(db);
            services.AddScoped<ApiExceptionFilter>();
            services.AddScoped<ValidateRequestModelAttribute>();
            services.AddScoped<EmailNotificationFilter>();
            services.AddScoped<RestrictApiAccessFilter>();
            services.AddTransient<IHashService, HashService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ISecurityRepository, SecurityRepository>();
            services.AddTransient<INotificationService, NotificationService>();
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtOptions));
            _secretKey = jwtAppSettingOptions["secretKey"];
            double validFor = double.Parse(jwtAppSettingOptions[nameof(JwtOptions.ValidFor)]);
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));

            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
                options.ValidFor = TimeSpan.FromMinutes(validFor);
            });

            services.Configure<WebApiSettings>(settings =>
                Configuration.GetSection(nameof(WebApiSettings)).Bind(settings));
            services.Configure<NotificationSettings>(settings =>
                Configuration.GetSection(nameof(NotificationSettings)).Bind(settings));

#if NETCOREAPP2_0 || NET462

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

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });
#endif

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Logger log = new LoggerConfiguration()
                .WriteTo.LiteDB($"Logs/log_{DateTime.Now.Date:yyyy-dd-M}.db")
                .CreateLogger();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog(log, true);
            loggerFactory.AddFile("Logs/Flubu-{Date}.txt");
#if NETCOREAPP1_1
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
#endif

#if NETCOREAPP2_0  || NET462
            app.UseAuthentication();
#endif

            app.UseMvc();
        }
    }
}
