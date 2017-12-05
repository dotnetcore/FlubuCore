using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Tasks.Nuget
{
    public class DownloadNugetPackageInUserRepositoryTask : TaskBase<int, DownloadNugetPackageInUserRepositoryTask>
    {
        public const string FlubuCachePathOverrideEnvVariableName = "FLUBU_CACHE";
        private const string NuGetDirectoryName = "NuGet";
        private readonly string _packageId;
        private readonly Version _packageVersion;

        public DownloadNugetPackageInUserRepositoryTask(string packageId, Version packageVersion = null)
        {
            _packageId = packageId;
            _packageVersion = packageVersion;
        }

        public static string NuGetPackagesCacheDir
        {
            get
            {
                var overrideValue = Environment.GetEnvironmentVariable(FlubuCachePathOverrideEnvVariableName);
                if (overrideValue == null)
                {
                    return Path.Combine(
                        IOExtensions.GetUserProfileFolder(),
                        Path.Combine(".flubu", NuGetDirectoryName));
                }

                return Path.Combine(overrideValue, NuGetDirectoryName);
            }
        }

        public NuGetCmdLineTask.NuGetVerbosity? Verbosity { get; set; }

        public string PackageSource { get; set; } = "https://www.nuget.org/api/v2/";

        public string ConfigFile { get; set; }

        public string PackageDirectory { get; private set; }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            var findPackageTask = new FindNuGetPackageInUserRepositoryTask(_packageId);
            findPackageTask.Execute(context);

            if (findPackageTask.PackageVersion != null && _packageVersion != null
                                                       && findPackageTask.PackageVersion > _packageVersion)
            {
                PackageDirectory = findPackageTask.PackageDirectory;
                return 0;
            }

            if (findPackageTask.PackageDirectory != null)
            {
                PackageDirectory = findPackageTask.PackageDirectory;
                return 0;
            }

            var task = new NuGetCmdLineTask("install")
                .WithArguments(_packageId)
                .WithArguments($"-Source {PackageSource}")
                .WithArguments("-NonInteractive")
                .WithArguments($"-OutputDirectory {NuGetPackagesCacheDir}");

            if (_packageVersion != null)
                task.WithArguments($"-Version {_packageVersion}");

            if (ConfigFile != null)
                task.WithArguments($"-ConfigFile {ConfigFile}");

            if (Verbosity.HasValue)
                task.Verbosity = Verbosity.Value;

            task.Execute(context);

            findPackageTask.Execute(context);
            PackageDirectory = findPackageTask.PackageDirectory;

            context.LogError(
                PackageDirectory == null
                    ? $"Something is wrong, after downloading it the NuGet package '{_packageId}' still could not be found."
                    : "Package downloaded to '{packageDirectory}'");

            return 0;
        }
    }
}