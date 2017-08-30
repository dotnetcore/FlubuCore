using FlubuCore.Scripting;
using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using System.IO;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FlubuCore.WebApi;

//#ass .\output\FlubuCore.dll
//#ass .\output\FlubuCore.WebApi\FlubuCore.WebApi.dll
//#ass .\output\FlubuCore.WebApi\FlubuCore.WebApi.Model.dll
//#ass .\output\lib\Newtonsoft.Json.dll
namespace Build
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
                .Do(PrepareWebApi);
        }

        public void PrepareWebApi(ITaskContext context)
        {
            DeploymentConfig config = null;
            var json = File.ReadAllText("DeploymentConfig.json");
            config = JsonConvert.DeserializeObject<DeploymentConfig>(json);

            IUserRepository repository = new UserRepository();
            var hashService = new HashService();
            repository.AddUser(new User
            {
                Username = config.Username,
                Password = hashService.Hash(config.Password)
            });
          
            context.Tasks().UpdateJsonFileTask(@".\FlubuCore.WebApi\appsettings.json")
                .Update(new KeyValuePair<string, JValue>("WebApiSettings.AllowScriptUpload", new JValue(config.AllowScriptUpload))).Execute(context);

            context.Tasks().CopyFileTask("Users.json", "FlubuCore.WebApi\\Users.json", true).Execute(context);
            context.Tasks().CopyDirectoryStructureTask("FlubuCore.Webapi", config.DeploymentPath, true).Execute(context);
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
