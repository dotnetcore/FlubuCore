using System;
using System.Globalization;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Nuget
{
    public class FindNuGetPackageInUserRepositoryTask : TaskBase<int, FindNuGetPackageInUserRepositoryTask>
    {
        private readonly string _packageId;
        private Version _packageVersion;
        private string _packageDirectory;

        public FindNuGetPackageInUserRepositoryTask(string packageId)
        {
            _packageId = packageId;
        }

        public string PackageId => _packageId;

        public Version PackageVersion => _packageVersion;

        public string PackageDirectory => _packageDirectory;

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _packageVersion = null;
            _packageDirectory = null;

            if (!Directory.Exists(DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir))
            {
                DoLogInfo($"Flubu NuGet user repository directory '{DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir}' does not exist.");
                return -1;
            }

            foreach (string directory in Directory.EnumerateDirectories(
                DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir,
                string.Format(CultureInfo.InvariantCulture, "{0}.*", _packageId)))
            {
                string localDirName = Path.GetFileName(directory);
                string versionStr = localDirName.Substring(_packageId.Length + 1);

                if (!Version.TryParse(versionStr, out var version))
                    continue;

                if (_packageVersion == null || version > _packageVersion)
                {
                    _packageVersion = version;
                    _packageDirectory = Path.Combine(
                        DownloadNugetPackageInUserRepositoryTask.NuGetPackagesCacheDir,
                        localDirName);
                }
            }

            DoLogInfo(_packageVersion != null
                ? $"Found NuGet package {_packageId} version {_packageVersion} in user repository ('{_packageDirectory}')"
                : $"No NuGet package {_packageId} in user repository (should be at '{_packageDirectory}')");

            return 0;
        }
    }
}
