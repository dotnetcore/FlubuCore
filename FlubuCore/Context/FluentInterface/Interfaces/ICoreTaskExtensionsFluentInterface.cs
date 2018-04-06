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
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            Action<PackageTask> action, params string[] projects);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string runtime, Action<PackageTask> packageTaskOptions, params string[] projects);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework, string project, Action<PackageTask> packageTaskOptions = null, string runtime = null);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, Action<PackageTask> packageTaskOptions = null, string runtime = null);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, Action<PackageTask> packageTaskOptions = null, string runtime = null);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        /// [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, Action<PackageTask> packageTaskOptions = null,
            string runtime = null);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5,
            Action<PackageTask> packageTaskOptions = null, string runtime = null);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, string project6,
            Action<PackageTask> action = null, string runtime = null);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, string project6,
            string project7, Action<PackageTask> packageTaskOptions = null, string runtime = null);

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="projects">The projects to test, defaults to the current directory.</param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(params string[] projects);

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="projects">The projects to test, defaults to the current directory</param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> taskAction = null, params string[] projects);

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="taskAction">The projects to test, defaults to the current directory</param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(string project = null, Action<DotnetTestTask> taskAction = null);

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> taskAction = null);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetRestore(params string[] projects);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> taskAction, params string[] projects);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> taskAction = null);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPublish(params string[] projects);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> taskAction, params string[] projects);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> taskAction = null);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPublish(string project, Action<DotnetPublishTask> taskAction = null);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="projects">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="projects">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> taskAction, params string[] projects);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> taskAction = null);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="workingFolder"></param>
        /// <param name="taskAction"></param>
        /// <param name="projects">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder = null, Action<DotnetBuildTask> taskAction = null, params string[] projects);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="project">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <param name="workingFolder"></param>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetBuild(string project = null, string workingFolder = null, Action<DotnetBuildTask> taskAction = null);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPack(params string[] projects);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> taskAction, params string[] projects);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> taskAction = null);

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> taskOptions, params string[] projects);

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetClean(string project, Action<DotnetCleanTask> taskOptions = null);

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> taskOptions = null);

        /// <summary>
        /// Pushes the nuget package to nuget server.
        /// </summary>
        /// <param name="nugetPackagePath">Path to .nupkg file</param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetNugetPush(string nugetPackagePath, Action<DotnetNugetPushTask> taskOptions = null);

        /// <summary>
        /// Updates the version in csproj / project.json file
        /// </summary>
        /// <param name="projectFiles"></param>
        /// <param name="additionalProps"></param>
        /// <returns></returns>
        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetAddEfMigration(string workingFolder, string migrationName = "default",  Action<ExecuteDotnetTask> taskOptions = null);

        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetRemoveEfMigration(string workingFolder, bool forceRemove = true, Action<ExecuteDotnetTask> taskOptions = null);

        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetEfUpdateDatabase(string workingFolder, Action<ExecuteDotnetTask> taskOptions = null);

        [Obsolete("CoreTaskExtensions fluent interface is obsolete all extensions were migrated to CoreTask  fluent interface with same name as now as extension methods or new tasks.", true)]
        ICoreTaskExtensionsFluentInterface DotnetEfDropDatabase(string workingFolder, Action<ExecuteDotnetTask> taskOptions = null);

        /// <summary>
        /// Moves back to target fluent interface.
        /// </summary>
        /// <returns></returns>
        ITargetFluentInterface BackToTarget();
    }
}
