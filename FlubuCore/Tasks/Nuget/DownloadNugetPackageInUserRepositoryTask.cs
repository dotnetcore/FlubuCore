using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Tasks.Nuget
{
    public class DownloadNugetPackageInUserRepositoryTask : TaskBase<int, DownloadNugetPackageInUserRepositoryTask>
    {
        public DownloadNugetPackageInUserRepositoryTask(string packageId, Version packageVersion = null)
        {
            this.packageId = packageId;
            this.packageVersion = packageVersion;
        }

        public NuGetCmdLineTask.NuGetVerbosity? Verbosity
        {
            get { return verbosity; }
            set { verbosity = value; }
        }

        public string PackageSource
        {
            get { return packageSource; }
            set { packageSource = value; }
        }

        public string ConfigFile
        {
            get { return configFile; }
            set { configFile = value; }
        }

        public string PackageDirectory
        {
            get { return packageDirectory; }
        }

        public const string FlubuCachePathOverrideEnvVariableName = "FLUBU_CACHE";
        private const string NuGetDirectoryName = "NuGet";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Nu")]
        public static string NuGetPackagesCacheDir
        {
            get
            {
                string overrideValue = Environment.GetEnvironmentVariable(FlubuCachePathOverrideEnvVariableName);
                if (overrideValue == null)
                    return Path.Combine(
                        IOExtensions.GetUserProfileFolder(),
                        Path.Combine(".flubu", NuGetDirectoryName));

                return Path.Combine(overrideValue, NuGetDirectoryName);
            }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            FindNuGetPackageInUserRepositoryTask findPackageTask = new FindNuGetPackageInUserRepositoryTask(packageId);
            findPackageTask.Execute(context);

            if (findPackageTask.PackageVersion != null && packageVersion != null
                && findPackageTask.PackageVersion > packageVersion)
            {
                packageDirectory = findPackageTask.PackageDirectory;
                return 0;
            }

            if (findPackageTask.PackageDirectory != null)
            {
                packageDirectory = findPackageTask.PackageDirectory;
                return 0;
            }

            NuGetCmdLineTask task = new NuGetCmdLineTask("install")
                .WithArguments(packageId)
                .WithArguments($"-Source {packageSource}")
                .WithArguments("-NonInteractive")
                .WithArguments($"-OutputDirectory {NuGetPackagesCacheDir}");

            if (packageVersion != null)
                task.WithArguments($"-Version {packageVersion.ToString()}");

            if (configFile != null)
                task.WithArguments($"-ConfigFile {configFile}");

            if (verbosity.HasValue)
                task.Verbosity = verbosity.Value;

            task.Execute(context);

            findPackageTask.Execute(context);
            packageDirectory = findPackageTask.PackageDirectory;

            if (packageDirectory == null)
                context.LogError($"Something is wrong, after downloading it the NuGet package '{packageId}' still could not be found.");
            else
                context.LogError("Package downloaded to '{packageDirectory}'");

            return 0;
        }

        private readonly string packageId;
        private readonly Version packageVersion;
        private string packageDirectory;
        private string configFile;
        private NuGetCmdLineTask.NuGetVerbosity? verbosity;
        private string packageSource = "https://www.nuget.org/api/v2/";
    }
}
