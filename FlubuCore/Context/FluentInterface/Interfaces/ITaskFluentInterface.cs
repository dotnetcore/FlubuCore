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

        /// <summary>
        /// Creates Nupkg file from nuspec file and publises it to the nuget server.
        /// </summary>
        /// <param name="packageId">Id of the nuget package.</param>
        /// <param name="nuspecFileName">Path to the nuspec file.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Runs the cooverage report generator tool
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <returns></returns>
        CoverageReportTask CoverageReportTask(params string[] inputFiles);

        /// <summary>
        /// Cleans the output of all projects in the solution.
        /// </summary>
        /// <returns></returns>
        CleanOutputTask CleanOutputTask();

        /// <summary>
        /// Task runs tests that are in specified project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        NUnitTask NUnitTaskForNunitV3(params string[] projectName);

        /// <summary>
        /// Task runs tests that are in specified project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        NUnitTask NUnitTaskForNunitV2(params string[] projectName);

        /// <summary>
        /// Task runs tests that are in specified project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        NUnitTask NUnitTaskByProjectName(params string[] projectName);

        /// <summary>
        /// Task runs tests that are in specified assembly..
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        NUnitTask NUnitTaskByAssemblyName(params string[] testAssemblyFileName);

        ReplaceTokensTask ReplaceTokensTask(string sourceFileName, string destinationFileName);

        /// <summary>
        /// Update's specified json.
        /// </summary>
        /// <param name="fileName">File name of the json to be updated.</param>
        /// <returns></returns>
        UpdateJsonFileTask UpdateJsonFileTask(string fileName);

        /// <summary>
        /// Task Fetches build version from file.
        /// </summary>
        /// <returns></returns>
        FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask();

        /// <summary>
        /// Task fetched build version from external soruce(appveryor).
        /// </summary>
        /// <returns></returns>
        FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask();

        /// <summary>
        /// Generate's common assembly info file. Information is taken from <see cref="IBuildPropertiesSession"/>
        /// </summary>
        /// <returns></returns>
        GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask();

        /// <summary>
        /// Task generate's common assembly info file. Information is taken from <see cref="IBuildPropertiesSession"/>
        /// </summary>
        /// <returns></returns>
        GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask(Version buildVersion);

        /// <summary>
        /// Task runs open cover to cobertuta tool.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        OpenCoverToCoberturaTask OpenCoverToCoberturaTask(string input, string output);

        /// <summary>
        // /Task runs open cover tool.
        /// </summary>
        /// <returns></returns>
        OpenCoverTask OpenCoverTask();

        /// <summary>
        /// Task unzip specified zip to specified locattion.
        /// </summary>
        /// <param name="zip">Zip file to be unziped</param>
        /// <param name="destionation">Destination path where zip content will be unziped.</param>
        /// <returns></returns>
        UnzipTask UnzipTask(string zip, string destionation);

        /// <summary>
        /// Internet information service specific tasks.
        /// </summary>
        /// <returns></returns>
        IIisTaskFluentInterface IisTasks();

        /// <summary>
        /// Task copies file to specified location.
        /// </summary>
        /// <param name="sourceFileName">File to be copied</param>
        /// <param name="destinationFileName"></param>
        /// <param name="overwrite">If <c>true</c> file is owerwriten if exists. Otherwise not</param>
        /// <returns></returns>
        CopyFileTask CopyFileTask(string sourceFileName, string destinationFileName, bool overwrite);

        /// <summary>
        /// /Task creates directory.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="forceRecreate">If <c>true</c> directory is deleted if it exists and then created again.</param>
        /// <returns></returns>
        CreateDirectoryTask CreateDirectoryTask(string directoryPath, bool forceRecreate);

        /// <summary>
        /// Task deletes directory.
        /// </summary>
        /// <param name="directoryPath">Path of the directoy to be deleted</param>
        /// <param name="failIfNotExists">If <c>true</c> task fails if excetpion. Otherwise not.</param>
        /// <returns></returns>
        DeleteDirectoryTask DeleteDirectoryTask(string directoryPath, bool failIfNotExists);

        /// <summary>
        /// Task deletes files
        /// </summary>
        /// <param name="directoryPath">Path of the directoy files to be deleted in</param>
        /// <param name="filePattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters but doesnt support regular expressions.</param>
        /// <param name="recursive">If <c>true</c> files in subdirectories is searched. Otherwise only in root directory.</param>
        /// <returns></returns>
        DeleteFilesTask DeleteFilesTask(string directoryPath, string filePattern, bool recursive);

        /// <summary>
        /// Merges multiple configuration files into one.
        /// </summary>
        /// <param name="outFile"></param>
        /// <returns></returns>
        MergeConfigurationTask MergeConfigurationTask(string outFile);

        /// <summary>
        /// Merges multiple configuration files into one.
        /// </summary>
        /// <param name="outFile"></param>
        /// <param name="sourceFiles"></param>
        /// <returns></returns>
        MergeConfigurationTask MergeConfigurationTask(string outFile, params string[] sourceFiles);

        /// <summary>
        /// Task Replaces text in file.
        /// </summary>
        /// <param name="sourceFile">File path.</param>
        /// <param name="tokens">Fist paramteter text to be replaced. Second parameter text to be replaced with</param>
        /// <returns></returns>
        ReplaceTextTask ReplaceTextTask(string sourceFile, params Tuple<string, string>[] tokens);
        
        /// <summary>
        /// Task updates xml. Xml elements can be added, updated or deleted.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        UpdateXmlFileTask UpdateXmlFileTask(string fileName);
    }
}
