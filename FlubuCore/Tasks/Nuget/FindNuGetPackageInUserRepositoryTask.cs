using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Nuget
{
    public class FindNuGetPackageInUserRepositoryTask : TaskBase<int, FindNuGetPackageInUserRepositoryTask>
    {
        public FindNuGetPackageInUserRepositoryTask(string packageId)
        {
            this.packageId = packageId;
        }

        public string PackageId
        {
            get { return packageId; }
        }

        public Version PackageVersion
        {
            get { return packageVersion; }
        }

        public string PackageDirectory
        {
            get { return packageDirectory; }
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            packageVersion = null;
            packageDirectory = null;

            if (!Directory.Exists(DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir))
            {
                context.LogInfo($"Flubu NuGet user repository directory '{DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir}' does not exist.");
                return -1;
            }

            foreach (string directory in Directory.EnumerateDirectories(
                DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir,
                string.Format(CultureInfo.InvariantCulture, "{0}.*", packageId)))
            {
                string localDirName = Path.GetFileName(directory);
                string versionStr = localDirName.Substring(packageId.Length + 1);

                Version version;
                if (!Version.TryParse(versionStr, out version))
                    continue;

                if (packageVersion == null || version > packageVersion)
                {
                    packageVersion = version;
                    packageDirectory = Path.Combine(
                        DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir,
                        localDirName);
                }
            }

            if (packageVersion != null)
                context.LogInfo($"Found NuGet package {packageId} version {packageVersion} in user repository ('{packageDirectory}')");
            else
                context.LogInfo($"No NuGet package {packageId} in user repository (should be at '{packageDirectory}')");

            return 0;
        }

        private readonly string packageId;
        private Version packageVersion;
        private string packageDirectory;
    }
}
