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

        ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> action = null, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetUnitTest(string project = null, Action<DotnetTestTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetRestore(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetPublish(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> action, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetPublish(string project, Action<DotnetPublishTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> action, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder = null, Action<DotnetBuildTask> action = null, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetBuild(string project = null, string workingFolder = null, Action<DotnetBuildTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetPack(params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> action, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> action, params string[] projects);

        ICoreTaskExtensionsFluentInterface DotnetClean(string project, Action<DotnetCleanTask> action = null);

        ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> action = null);

        ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        ITargetFluentInterface BackToTarget();
    }
}
