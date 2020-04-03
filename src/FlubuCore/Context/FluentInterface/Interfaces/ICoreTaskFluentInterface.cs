using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ICoreTaskFluentInterface
    {
        /// <summary>
        /// Task specific for Linux operating system.
        /// </summary>
        /// <returns></returns>
        ILinuxTaskFluentInterface LinuxTasks();

        /// <summary>
        /// Executes specified dotnet command.
        /// </summary>
        /// <param name="command">Commdand to execute.</param>
        /// <returns></returns>
        ExecuteDotnetTask ExecuteDotnetTask(string command);

        /// <summary>
        /// Executes specified dotnet command.
        /// </summary>
        /// <param name="command">Commdand to execute.</param>
        /// <returns></returns>
        ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command);

        /// <summary>
        /// Updates the version in csproj or project.json file.
        /// </summary>
        /// <param name="files">The project files to update.</param>
        /// <returns></returns>
        UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files);

        /// <summary>
        /// Restores the dependencies and tools for a given application / project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="workingFolder"></param>
        /// <returns></returns>
        DotnetRestoreTask Restore(string projectName = null, string workingFolder = null);

        /// <summary>
        /// compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="workingFolder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        DotnetPublishTask Publish(
            string projectName = null,
            string workingFolder = null,
            string configuration = null);

        /// <summary>
        /// Builds a project and all of its dependencies.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="workingFolder"></param>
        /// <returns></returns>
        DotnetBuildTask Build(string projectName = null, string workingFolder = null);

        /// <summary>
        /// command builds the project and creates NuGet packages. The result of this command is a NuGet package.
        /// </summary>
        /// <returns></returns>
        DotnetPackTask Pack();

        /// <summary>
        /// Runs tests using a test runner specified in the project.json / csproj.
        /// </summary>
        /// <returns></returns>
        DotnetTestTask Test();

        /// <summary>
        /// Cleans the output of a project.
        /// </summary>
        /// <returns></returns>
        DotnetCleanTask Clean();

        /// <summary>
        /// Builds the specified targets in the project file.
        /// </summary>
        /// <returns></returns>
        DotnetMsBuildTask MsBuild();

        /// <summary>
        /// Pushes the nuget package to nuget server.
        /// </summary>
        /// <param name="packagePath">Path to .nupkg file.</param>
        /// <returns></returns>
        DotnetNugetPushTask NugetPush(string packagePath);

        IToolsFluentInterface Tool();

        /// <summary>
        /// Coverlet is a cross platform code coverage library for .NET Core, with support for line, branch and method coverage.
        /// more info at: https://github.com/tonerdo/coverlet.
        /// </summary>
        /// <param name="assembly">Path to the test assembly.</param>
        /// <returns></returns>
        CoverletTask CoverletTask(string assembly);
    }
}
