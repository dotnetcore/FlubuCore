using FlubuCore.Scripting;
using System;
using System.Collections.Generic;
using FlubuCore.Context;
using System.IO;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FlubuCore.WebApi;
using LiteDB;

//#ass  .\lib\System.Reflection.TypeExtensions.dll
//#ass .\FlubuCore.WebApi\FlubuCore.WebApi.dll
//#ass .\FlubuCore.WebApi\FlubuCore.WebApi.Model.dll
//#ass .\lib\Newtonsoft.Json.dll
//#ass .\lib\LiteDB.dll

namespace DeploymentScript
{
    public class DeploymentScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            session.CreateTarget("Deploy")
                .SetDescription("Deploys flubu web api")
                .SetAsDefault()
                .Do(DeployWebApi);
        }

        public void DeployWebApi(ITaskContext context)
        {
            DeploymentConfig config = null;
            var json = File.ReadAllText("DeploymentConfig.json");
            config = JsonConvert.DeserializeObject<DeploymentConfig>(json);
            ValidateDeploymentConfig(config);
            using (var db = new LiteRepository("FileName=database.db"))
            {

                IUserRepository repository = new UserRepository(db);
                var hashService = new HashService();

                repository.AddUser(new User
                {
                    Username = config.Username,
                    Password = hashService.Hash(config.Password)
                });
            }

            context.Tasks().UpdateJsonFileTask(@".\FlubuCore.WebApi\appsettings.json")
                .Update(new KeyValuePair<string, JValue>("WebApiSettings.AllowScriptUpload", new JValue(config.AllowScriptUpload))).Execute(context);

            context.Tasks().UpdateJsonFileTask(@".\FlubuCore.WebApi\appsettings.json")
                .Update("JwtOptions.SecretKey", GenerateRandomString(30)).Execute(context);

            context.Tasks().CopyFileTask("Database.db", "FlubuCore.WebApi\\Database.db", true).Execute(context);

            context.Tasks().CopyDirectoryStructureTask("FlubuCore.Webapi", config.DeploymentPath, true).Execute(context);
            context.Tasks().CreateDirectoryTask(config.DeploymentPath + "\\Packages", false).Execute(context);
            context.Tasks().CreateDirectoryTask(config.DeploymentPath + "\\Scripts", false).Execute(context);
        }

        private static void ValidateDeploymentConfig(DeploymentConfig config)
        {
            if (string.IsNullOrEmpty(config.DeploymentPath))
            {
                throw new ArgumentException("DeploymentPath must not be empty in deployment config.");
            }

            if (string.IsNullOrEmpty(config.Username))
            {
                throw new ArgumentException("Username must not be empty in deployment config.");
            }

            if (string.IsNullOrEmpty(config.Password))
            {
                throw new ArgumentException("Password must not be empty in deployment config.");
            }
        }

        private string GenerateRandomString(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789<>][,.;{}>?!@$%^&*()_-=+|";
            var stringChars = new char[size];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

    }

    public class DeploymentConfig
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool AllowScriptUpload { get; set; }

        public string DeploymentPath { get; set; }
    }
}
