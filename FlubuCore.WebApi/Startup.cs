using System;
using System.IO;
using System.Text;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        public Startup(IHostingEnvironment env)
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
            // Add framework services.
            services.AddMvc(options =>
                {
                    options.ModelValidatorProviders.Clear();
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
            ConfigureSwagger(services);
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
            ConfigureAuthentication(app);

            app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
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

        private void ConfigureAuthentication(IApplicationBuilder app)
        {
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

#if NETCOREAPP2_0 || NETCOREAPP2_1 || NET462
            app.UseAuthentication();
#endif
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
            services.Configure<NotificationSettings>(settings =>
                Configuration.GetSection(nameof(NotificationSettings)).Bind(settings));

#if NETCOREAPP2_0 || NETCOREAPP2_1 || NET462
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
    }
}
