using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public string BasePath
        {
            get { return _basePath; }
            set { _basePath = value; }
        }

        public string NuGetServerUrl
        {
            get { return _nuGetServerUrl; }
            set { _nuGetServerUrl = value; }
        }

        public bool AllowPushOnInteractiveBuild
        {
            get { return _allowPushOnInteractiveBuild; }
            set { _allowPushOnInteractiveBuild = value; }
        }

        public void ForApiKeyUse(string apiKey)
        {
            _apiKeyFunc = c => apiKey;
        }

        public void ForApiKeyUseEnvironmentVariable(string variableName = DefaultNuGetApiKeyEnvVariable)
        {
            _apiKeyFunc = c => FetchNuGetApiKeyFromEnvVariable(c, variableName);
        }

        public void ForApiKeyUseFile(string fileName)
        {
            _apiKeyFunc = c => FetchNuGetApiKeyFromLocalFile(c, fileName);
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            FullPath packagesDir = new FullPath(context.Properties.Get(BuildProps.ProductRootDir, "."));
            packagesDir = packagesDir.CombineWith(context.Properties.Get<string>(BuildProps.BuildDir));

            FileFullPath destNuspecFile = packagesDir.AddFileName("{0}.nuspec", _packageId);

            context.LogInfo(string.Format("Preparing the {0} file", destNuspecFile));
            ReplaceTokensTask task = new ReplaceTokensTask(
                _nuspecFileName,
                destNuspecFile.ToString());
            task.AddTokenValue("version", context.GetBuildVersion().ToString());
            task.ExecuteVoid(context);

            // package it
            context.LogInfo("Creating a NuGet package file");
            string nugetWorkingDir = destNuspecFile.Directory.ToString();
            NuGetCmdLineTask nugetTask = new NuGetCmdLineTask("pack", nugetWorkingDir);
            nugetTask.Verbosity = NuGetCmdLineTask.NuGetVerbosity.Detailed;
            nugetTask
                .AddArgument(destNuspecFile.FileName);

            if (_basePath != null)
                nugetTask.AddArgument("-BasePath").AddArgument(_basePath);

            nugetTask
                .ExecuteVoid(context);

            string nupkgFileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}.nupkg",
                _packageId,
                context.GetBuildVersion());
            context.LogInfo(string.Format("NuGet package file {0} created", nupkgFileName));

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

            nugetTask = new NuGetCmdLineTask("push", nugetWorkingDir);
            nugetTask.Verbosity = NuGetCmdLineTask.NuGetVerbosity.Detailed;
            nugetTask.ApiKey = apiKey;
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
                context.Fail(string.Format("NuGet API key file ('{0}') does not exist, cannot publish the package.", fileName), 1);
                return null;
            }

            return File.ReadAllText(fileName).Trim();
        }

        private static string FetchNuGetApiKeyFromEnvVariable(ITaskContextInternal context, string environmentVariableName = DefaultNuGetApiKeyEnvVariable)
        {
            string apiKey = Environment.GetEnvironmentVariable(environmentVariableName);

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Fail(string.Format("NuGet API key environment variable ('{0}') does not exist, cannot publish the package.", environmentVariableName), 1);
                return null;
            }

            return apiKey;
        }
    }
}
