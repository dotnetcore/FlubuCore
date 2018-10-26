using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.WebApi;
using FlubuCore.WebApi.Infrastructure;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//#ass  .\lib\netstandard.dll
//#ass  .\lib\System.Reflection.TypeExtensions.dll
//#ass .\FlubuCore.WebApi\FlubuCore.WebApi.dll
//#ass .\FlubuCore.WebApi\FlubuCore.WebApi.Model.dll
//#ass .\lib\Newtonsoft.Json.dll
//#ass .\lib\LiteDB.dll
//#ass .\lib\System.Security.Cryptography.Algorithms.dll

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
            string connectionString;
            if (!string.IsNullOrWhiteSpace(config.LiteDbConnectionString))
            {
                connectionString = config.LiteDbConnectionString;
            }
            else
            {
                var liteDbPassword = GenerateRandomSecureString(15);
                connectionString = $"FileName=database.db; Password={liteDbPassword}";
            }

            bool createDb = false;
            var dbFileName = Files.GetFileNameFromConnectionString(connectionString);
            var isPathRooted = Path.IsPathRooted(dbFileName);

            if (!config.CopyOnlyBinaries)
            {
                if (config.RecreateDatabase)
                {
                    createDb = true;
                }
                else
                {
                    if (isPathRooted)
                    {
                        if (!File.Exists(dbFileName))
                        {
                            createDb = true;
                        }
                    }
                    else
                    {
                        var dbPath = Path.Combine(config.DeploymentPath, dbFileName);
                        if (!File.Exists(dbPath))
                        {
                            createDb = true;
                        }
                    }
                }

                if (createDb)
                {
                    File.Delete(dbFileName);
                    using (var db = new LiteRepository(connectionString))
                    {
                        IUserRepository repository = new UserRepository(db);
                        var hashService = new HashService();

                        repository.AddUser(new User
                        {
                            Username = config.Username,
                            Password = hashService.Hash(config.Password),
                        });
                    }

                    if (!isPathRooted)
                    {
                        var outputDbFilePath = Path.Combine("FlubuCore.WebApi", dbFileName);

                        context.Tasks().CopyFileTask(dbFileName, outputDbFilePath, true)
                            .Execute(context);
                    }
                }
                else
                {
                    ////Delete old db file if it exists so it doesnt rewrite database at deployed location.
                    var outputDbFilePath = Path.Combine("FlubuCore.WebApi", dbFileName);
                    if (File.Exists(outputDbFilePath))
                    {
                        File.Delete(outputDbFilePath);
                    }
                }

                context.Tasks().UpdateJsonFileTask(@".\FlubuCore.WebApi\appsettings.json")
                    .Update(new KeyValuePair<string, JValue>("FlubuConnectionStrings.LiteDbConnectionString",
                        new JValue(connectionString))).Execute(context);

                context.Tasks().UpdateJsonFileTask(@".\FlubuCore.WebApi\appsettings.json")
                    .Update(new KeyValuePair<string, JValue>("WebApiSettings.AllowScriptUpload",
                        new JValue(config.AllowScriptUpload))).Execute(context);

                context.Tasks().UpdateJsonFileTask(@".\FlubuCore.WebApi\appsettings.json")
                    .Update("JwtOptions.SecretKey", GenerateRandomString(30)).Execute(context);
                context.Tasks().CreateDirectoryTask(config.DeploymentPath + "\\Packages", false).Execute(context);
                context.Tasks().CreateDirectoryTask(config.DeploymentPath + "\\Scripts", false).Execute(context);
            }
            else
            {
                ////Delete old db file if it exists so it doesnt rewrite database at deployed location.
                var outputDbFilePath = Path.Combine("FlubuCore.WebApi", dbFileName);
                if (File.Exists(outputDbFilePath))
                {
                    File.Delete(outputDbFilePath);
                }
            }

            context.Tasks().CopyDirectoryStructureTask("FlubuCore.Webapi", config.DeploymentPath, true)
                .Execute(context);
        }

        private static void ValidateDeploymentConfig(DeploymentConfig config)
        {
            if (string.IsNullOrEmpty(config.DeploymentPath))
            {
                throw new ArgumentException("DeploymentPath must not be empty in deployment config.");
            }

            System.IO.FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(config.DeploymentPath);
            }
            catch (ArgumentException)
            {
            }
            catch (System.IO.PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }

            if (ReferenceEquals(fi, null))
            {
                throw new ArgumentException("DeploymentPath is not a legal path. Did u use double '\\'?");
            }
            else
            {

            }

            if (config.CopyOnlyBinaries)
            {
                return;
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

            return new string(stringChars);
        }

        private string GenerateRandomSecureString(int length,
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");

            if (string.IsNullOrEmpty(allowedChars))
                throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
                throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.",
                    byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }

                return result.ToString();
            }
        }

        public class DeploymentConfig
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public bool RecreateDatabase { get; set; }

            public bool CopyOnlyBinaries { get; set; }

            public bool AllowScriptUpload { get; set; }

            public string DeploymentPath { get; set; }

            public string LiteDbConnectionString { get; set; }
        }
    }
}
