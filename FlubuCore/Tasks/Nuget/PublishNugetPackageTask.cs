using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Nuget
{
    public class PublishNuGetPackageTask : TaskBase<int>
    {
        public const string DefaultNuGetApiKeyEnvVariable = "NuGetOrgApiKey";

        public const string DefaultApiKeyFileName = "private/nuget.org-api-key.txt";

        private readonly string _packageId;

        private readonly string _nuspecFileName;

        private bool _allowPushOnInteractiveBuild;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private string _nuGetServerUrl;

        private Func<ITaskContextInternal, string> _apiKeyFunc;

        private string _basePath;

        public PublishNuGetPackageTask(string packageId, string nuspecFileName)
        {
            _packageId = packageId;
            _nuspecFileName = nuspecFileName;
        }

        /// <summary>
        /// nuget base path argument to be added.
        /// </summary>
        public string BasePath
        {
            get { return _basePath; }
            set { _basePath = value; }
        }

        /// <summary>
        /// Nuget server url package will be pushed to.
        /// </summary>
        public PublishNuGetPackageTask NugetServerUrl(string url)
        {
            _nuGetServerUrl = url;
            return this;
        }

        public PublishNuGetPackageTask PushOnInteractiveBuild()
        {
            _allowPushOnInteractiveBuild = true;
            return this;
        }

        /// <summary>
        /// Nuget server Api key.
        /// </summary>
        /// <param name="apiKey"></param>
        public PublishNuGetPackageTask ForApiKeyUse(string apiKey)
        {
            _apiKeyFunc = c => apiKey;
            return this;
        }

        /// <summary>
        /// Name of the enviroment variable to use to get api key. 
        /// </summary>
        /// <param name="variableName"></param>
        public PublishNuGetPackageTask ForApiKeyUseEnvironmentVariable(string variableName = DefaultNuGetApiKeyEnvVariable)
        {
            _apiKeyFunc = c => FetchNuGetApiKeyFromEnvVariable(c, variableName);
            return this;
        }

        /// <summary>
        /// Path to the file that contains api key.
        /// </summary>
        /// <param name="fileName"></param>
        public PublishNuGetPackageTask ForApiKeyUseFile(string fileName)
        {
            _apiKeyFunc = c => FetchNuGetApiKeyFromLocalFile(c, fileName);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            FullPath packagesDir = new FullPath(context.Properties.Get(BuildProps.ProductRootDir, "."));
            packagesDir = packagesDir.CombineWith(context.Properties.Get<string>(BuildProps.BuildDir));

            FileFullPath destNuspecFile = packagesDir.AddFileName("{0}.nuspec", _packageId);

            context.LogInfo($"Preparing the {destNuspecFile} file");
            ReplaceTokensTask task = new ReplaceTokensTask(
                _nuspecFileName,
                destNuspecFile.ToString());
            task.AddTokenValue("version", context.Properties.GetBuildVersion().ToString());
            task.ExecuteVoid(context);

            // package it
            context.LogInfo("Creating a NuGet package file");
            string nugetWorkingDir = destNuspecFile.Directory.ToString();
            NuGetCmdLineTask nugetTask = new NuGetCmdLineTask("pack", nugetWorkingDir)
            {
                Verbosity = NuGetCmdLineTask.NuGetVerbosity.Detailed
            };

            nugetTask.AddArgument(destNuspecFile.FileName);

            if (_basePath != null)
                nugetTask.AddArgument("-BasePath").AddArgument(_basePath);

            nugetTask.ExecuteVoid(context);

            string nupkgFileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}.nupkg",
                _packageId,
                context.Properties.GetBuildVersion());
            context.LogInfo($"NuGet package file {nupkgFileName} created");

            // do not push new packages from a local build
            if (context.IsInteractive && !_allowPushOnInteractiveBuild)
                return 1;

            if (_apiKeyFunc == null)
                throw new InvalidOperationException("NuGet API key was not provided");

            string apiKey = _apiKeyFunc(context);
            if (apiKey == null)
                return 1;

            // publish the package file
            context.LogInfo("Pushing the NuGet package to the repository");

            nugetTask = new NuGetCmdLineTask("push", nugetWorkingDir)
            {
                Verbosity = NuGetCmdLineTask.NuGetVerbosity.Detailed,
                ApiKey = apiKey
            };

            if (_nuGetServerUrl != null)
                nugetTask.AddArgument("-Source").AddArgument(_nuGetServerUrl);

            nugetTask
                .AddArgument(nupkgFileName)
                .ExecuteVoid(context);

            return 0;
        }

        private static string FetchNuGetApiKeyFromLocalFile(ITaskContextInternal context, string fileName = DefaultApiKeyFileName)
        {
            if (!File.Exists(fileName))
            {
                context.Fail($"NuGet API key file ('{fileName}') does not exist, cannot publish the package.", 1);
                return null;
            }

            return File.ReadAllText(fileName).Trim();
        }

        private static string FetchNuGetApiKeyFromEnvVariable(ITaskContextInternal context, string environmentVariableName = DefaultNuGetApiKeyEnvVariable)
        {
            string apiKey = Environment.GetEnvironmentVariable(environmentVariableName);

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Fail($"NuGet API key environment variable ('{environmentVariableName}') does not exist, cannot publish the package.", 1);
                return null;
            }

            return apiKey;
        }
    }
}
