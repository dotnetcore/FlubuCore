using System;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ICoreTaskExtensionsFluentInterface
    {
        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/publish</param>
        /// <param name="projects">Folders to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        PackageTask CreateZipPackageFromProjects(string zipPrefix, string targetFramework, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetUnitTest(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action = null, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetPublish(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder, params string[] projects);

        ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        ITargetFluentInterface BackToTarget();
    }
}
