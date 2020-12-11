using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Nuget
{
    public class PublishNuGetPackageTask : TaskBase<int, PublishNuGetPackageTask>
    {
        public const string DefaultNuGetApiKeyEnvVariable = "NuGetOrgApiKey";

        public const string DefaultApiKeyFileName = "private/nuget.org-api-key.txt";

        private readonly string _packageId;

        private readonly string _nuspecFileName;

        private bool _skipPushOnLocalBuild = false;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private string _nuGetServerUrl;

        private Func<ITaskContextInternal, string> _apiKeyFunc;
        private string _description;

        public PublishNuGetPackageTask(string packageId, string nuspecFileName)
        {
            _packageId = packageId;
            _nuspecFileName = nuspecFileName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Pushes NuGet package {_packageId} to NuGet server '{_nuGetServerUrl}'";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// nuget base path argument to be added.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Nuget server url package will be pushed to.
        /// </summary>
        public PublishNuGetPackageTask NugetServerUrl(string url)
        {
            _nuGetServerUrl = url;
            return this;
        }

        /// <summary>
        /// If applied pushing packages to nuget repository is disabled on local build.
        /// </summary>
        /// <returns></returns>
        public PublishNuGetPackageTask SkipPushOnLocalBuild()
        {
            _skipPushOnLocalBuild = true;
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
            packagesDir = packagesDir.CombineWith(context.Properties.Get<string>(DotNetBuildProps.BuildDir));

            FileFullPath destNuspecFile = packagesDir.AddFileName("{0}.nuspec", _packageId);

            DoLogInfo($"Preparing the {destNuspecFile} file");

            new ReplaceTokensTask(_nuspecFileName)
                .Replace("version", context.Properties.GetBuildVersion().BuildVersionWithQuality(3))
                .UseToken("$")
                .ToDestination(destNuspecFile.ToString())
                .ExecuteVoid(context);

            // package it
            DoLogInfo("Creating a NuGet package file");
            string nugetWorkingDir = destNuspecFile.Directory.ToString();
            NuGetCmdLineTask nugetTask = new NuGetCmdLineTask("pack", nugetWorkingDir)
            {
                Verbosity = NuGetCmdLineTask.NuGetVerbosity.Detailed
            };

            nugetTask.WithArguments(destNuspecFile.FileName);

            if (BasePath != null)
                nugetTask.WithArguments("-BasePath", BasePath);

            nugetTask.ExecuteVoid(context);

            string nupkgFileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}.nupkg",
                _packageId,
                context.Properties.GetBuildVersion().BuildVersionWithQuality(3));
            DoLogInfo($"NuGet package file {nupkgFileName} created");

            // do not push new packages from a local build
            if (context.BuildServers().IsLocalBuild && _skipPushOnLocalBuild)
            {
                context.LogInfo("pushing package on local build is disabled in build script...Skiping.");
                return 1;
            }

            if (_apiKeyFunc == null)
                throw new InvalidOperationException("NuGet API key was not provided");

            string apiKey = _apiKeyFunc(context);
            if (apiKey == null)
                return 1;

            // publish the package file
            DoLogInfo("Pushing the NuGet package to the repository");

            nugetTask = new NuGetCmdLineTask("push", nugetWorkingDir)
            {
                Verbosity = NuGetCmdLineTask.NuGetVerbosity.Detailed,
                ApiKey = apiKey
            };

            if (_nuGetServerUrl != null)
                nugetTask.WithArguments("-Source", _nuGetServerUrl);

            nugetTask
                .WithArguments(nupkgFileName)
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
