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


        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="projects">The projects to test, defaults to the current directory.</param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(params string[] projects);

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="projects">The projects to test, defaults to the current directory</param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> action = null, params string[] projects);

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="action">The projects to test, defaults to the current directory</param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(string project = null, Action<DotnetTestTask> action = null);


        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> action = null);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetRestore(params string[] projects);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action, params string[] projects);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action = null);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPublish(params string[] projects);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> action, params string[] projects);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> action = null);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPublish(string project, Action<DotnetPublishTask> action = null);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="projects">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="action"></param>
        /// <param name="projects">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> action, params string[] projects);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> action = null);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="workingFolder"></param>
        /// <param name="action"></param>
        /// <param name="projects">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder = null, Action<DotnetBuildTask> action = null, params string[] projects);

        /// <summary>
        /// Builds a project and all of its dependencies
        /// </summary>
        /// <param name="project">The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.</param>
        /// <param name="workingFolder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetBuild(string project = null, string workingFolder = null, Action<DotnetBuildTask> action = null);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPack(params string[] projects);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> action, params string[] projects);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> action = null);

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> action, params string[] projects);

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetClean(string project, Action<DotnetCleanTask> action = null);

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> action = null);

        /// <summary>
        /// Pushes the nuget package to nuget server.
        /// </summary>
        /// <param name="nugetPackagePath">Path to .nupkg file</param>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface DotnetNugetPush(string nugetPackagePath, Action<DotnetNugetPushTask> action = null);

        /// <summary>
        /// Updates the version in csproj / project.json file
        /// </summary>
        /// <param name="projectFiles"></param>
        /// <param name="additionalProps"></param>
        /// <returns></returns>
        ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        /// <summary>
        /// Moves back to target fluent interface.
        /// </summary>
        /// <returns></returns>
        ITargetFluentInterface BackToTarget();
    }
}
