using System;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.Nuget;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITaskFluentInterface
    {
        /// <summary>
        /// Task runs the specified program.
        /// </summary>
        /// <param name="programToExecute">The program to execute.</param>
        /// <returns></returns>
        IRunProgramTask RunProgramTask(string programToExecute);

        /// <summary>
        /// Copies the specified directory to the specified destination.
        /// </summary>
        /// <param name="sourcePath">Path of the source directory</param>
        /// <param name="destinationPath">Path where directory will be copied to.</param>
        /// <param name="overwriteExisting">If <c>True</c> directory on the destination path is overwriten if it exists. Otherwise not.</param>
        /// <returns></returns>
        CopyDirectoryStructureTask CopyDirectoryStructureTask(string sourcePath, string destinationPath, bool overwriteExisting);

        NuGetCmdLineTask NuGetCmdLineTask(string command, string workingDirectory = null);

        PublishNuGetPackageTask PublishNuGetPackageTask(string packageId, string nuspecFileName);

        /// <summary>
        /// Task Packages specified directories and files into specified directory or zip file.
        /// </summary>
        /// <param name="destinationRootDir">The destination directory where the package will be created.</param>
        /// <returns></returns>
        PackageTask PackageTask(string destinationRootDir);

        /// <summary>
        /// Task compiles the solution with MSBuild. Solution and build configuration is retrived from <see cref="IBuildPropertiesSession"/>.
        /// </summary>
        /// <returns></returns>
        CompileSolutionTask CompileSolutionTask();

        /// <summary>
        /// Task compiles specified solution with MsBuild
        /// </summary>
        /// <param name="solutionFileName">The file name of the solution to be compiled</param>
        /// <param name="buildConfiguration">The build configuration solution will be compiled in(Release, Debug...)</param>
        /// <returns></returns>
        CompileSolutionTask CompileSolutionTask(string solutionFileName, string buildConfiguration);

        /// <summary>
        /// Task load's solution information to the <see cref="TaskContextSession"/> solution file name is retieved from <see cref="TaskContextSession"/>
        /// </summary>
        LoadSolutionTask LoadSolutionTask();

        /// <summary>
        /// Task load's specified solution information to the <see cref="TaskContextSession"/>
        /// </summary>
        /// <param name="solutionFileName">The solution file name of the solution to be loaded.</param>
        LoadSolutionTask LoadSolutionTask(string solutionFileName);

        CoverageReportTask CoverageReportTask(params string[] inputFiles);

        CleanOutputTask CleanOutputTask();

        /// <summary>
        /// Task runs tests that are in specified project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        NUnitTask NUnitTaskForNunitV3(params string[] projectName);

        NUnitTask NUnitTaskForNunitV2(params string[] projectName);

        NUnitTask NUnitTaskByProjectName(params string[] projectName);

        NUnitTask NUnitTaskByAssemblyName(params string[] testAssemblyFileName);

        ReplaceTokensTask ReplaceTokensTask(string sourceFileName, string destinationFileName);

        UpdateJsonFileTask UpdateJsonFileTask(string fileName);

        FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask();

        FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask();

        GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask();

        GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask(Version buildVersion);

        OpenCoverToCoberturaTask OpenCoverToCoberturaTask(string input, string output);

        OpenCoverTask OpenCoverTask();

        UnzipTask UnzipTask(string zip, string destionation);

        IIisTaskFluentInterface IisTasks();

        CopyFileTask CopyFileTask(string sourceFileName, string destinationFileName, bool overwrite);

        CreateDirectoryTask CreateDirectoryTask(string directoryPath, bool forceRecreate);

        DeleteDirectoryTask DeleteDirectoryTask(string directoryPath, bool failIfNotExists);

        DeleteFilesTask DeleteFilesTask(string directoryPath, string filePattern, bool recursive);

        MergeConfigurationTask MergeConfigurationTask(string outFile);

        MergeConfigurationTask MergeConfigurationTask(string outFile, params string[] sourceFiles);

        ReplaceTextTask ReplaceTextTask(string sourceFile, params Tuple<string, string>[] tokens);
    }
}
