using System;
using System.IO;
using System.Text;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
#if NETCOREAPP3_1
    using Microsoft.Extensions.Hosting;
#else
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Logger = Serilog.Core.Logger;

namespace FlubuCore.WebApi
{
    public class Startup
    {
        private string _secretKey;

        private SymmetricSecurityKey _signingKey;

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
#if NETCOREAPP3_1
                     options.EnableEndpointRouting = false;
#endif
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = 1017483648;
                x.MultipartBodyLengthLimit = 1017483648;
            });

            services
                .AddCoreComponentsForWebApi(Configuration)
                .AddCommandComponentsForWebApi()
                .AddScriptAnalyserForWebApi()
                .AddTasksForWebApi();

            ConfigureAuthenticationServices(services);
#if !NETCOREAPP3_1
            ConfigureSwagger(services);
 #endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
        {
            Logger log = new LoggerConfiguration()
                .WriteTo.LiteDB($"Logs/log_{DateTime.Now.Date:yyyy-dd-M}.db")
                .CreateLogger();

            loggerFactory.AddSerilog(log, true);
            loggerFactory.AddFile("Logs/Flubu-{Date}.txt");
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseStaticFiles();
#if !NETCOREAPP3_1
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
#endif
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "A1 API",
                    Description = "A1 API",
                    TermsOfService = "None",
                });

                options.CustomSchemaIds(x => x.FullName);
                var basePath = AppContext.BaseDirectory;
                var webApifilePath = Path.Combine(basePath, "FlubuCore.WebApi.xml");
                var modelfilePath = Path.Combine(basePath, "FlubuCore.WebApi.Model.xml");
                options.IncludeXmlComments(webApifilePath);
                options.IncludeXmlComments(modelfilePath);
                options.DescribeAllEnumsAsStrings();
                options.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme()
                    {
                        In = "header",
                        Description =
                            "Please insert JWT with Bearer into field. Example value(Enter token without braces.): Bearer {JwtToken} ",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
            });
        }

        private void ConfigureAuthenticationServices(IServiceCollection services)
        {
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
            services.Configure<WebAppSettings>(settings =>
                Configuration.GetSection(nameof(WebAppSettings)).Bind(settings));
            services.Configure<NotificationSettings>(settings =>
                Configuration.GetSection(nameof(NotificationSettings)).Bind(settings));

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

             services.AddAuthentication(o =>
                {
                    o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Login");
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                });
        }
    }
}
