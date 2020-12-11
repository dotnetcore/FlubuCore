using System;
using System.Collections.Generic;
using System.Net.Http;
using FlubuCore.Tasks.Chocolatey;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.FlubuWebApi;
using FlubuCore.Tasks.MsSql;
using FlubuCore.Tasks.Nuget;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Testing.Xunit;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Utils;
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
        ///  Copies a directory tree from the source to the destination.
        /// </summary>
        /// <param name="sourcePath">Path of the source directory.</param>
        /// <param name="destinationPath">Path where directory will be copied to.</param>
        /// <param name="overwriteExisting">If <c>True</c> directory on the destination path is overwriten if it exists. Otherwise not.</param>
        /// <returns></returns>
        CopyDirectoryStructureTask CopyDirectoryStructureTask(string sourcePath, string destinationPath, bool overwriteExisting);

        /// <summary>
        /// Creates new <see cref="HttpClient"/> or get's existing one. Depending on the base url.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="baseUrl">The base url.</param>
        /// <returns></returns>
        HttpClient CreateHttpClient(string baseUrl);

        /// <summary>
        /// Execute NuGet command line tool.
        /// </summary>
        /// <param name="command">The nuget command to be executed. See nuget.exe help for command list.</param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
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
        /// Task compiles specified solution with MsBuild.
        /// </summary>
        /// <param name="solutionFileName">The file name of the solution to be compiled.</param>
        /// <param name="buildConfiguration">The build configuration solution will be compiled in(Release, Debug...)</param>
        /// <returns></returns>
        CompileSolutionTask CompileSolutionTask(string solutionFileName, string buildConfiguration);

        /// <summary>
        /// Chocolatey specific tasks. https://chocolatey.org/.
        /// </summary>
        /// <returns></returns>
        Chocolatey Chocolatey();

        /// <summary>
        /// Task load's solution information to the <see cref="BuildPropertiesSession"/> <see cref="BuildProps.Solution"/> solution file name is retieved from <see cref="BuildPropertiesSession"/>.
        /// </summary>
        LoadSolutionTask LoadSolutionTask();

        /// <summary>
        /// Task load's specified vs solution information to the <see cref="BuildPropertiesSession"/> <see cref="BuildProps.Solution"/>.
        /// </summary>
        /// <param name="solutionFileName">The solution file name of the solution to be loaded.</param>
        LoadSolutionTask LoadSolutionTask(string solutionFileName);

        /// <summary>
        /// Runs the cooverage report generator tool.
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <returns></returns>
        CoverageReportTask CoverageReportTask(params string[] inputFiles);

        /// <summary>
        /// Cleans the output directories of all projects in the solution.
        /// </summary>
        /// <returns></returns>
        CleanOutputTask CleanOutputTask();

        /// <summary>
        /// Task runs NUnit tests that are in specified project.
        /// </summary>
        /// <param name="cmdOptions">Adds default command line options for specified NUnit version.</param>
        /// <param name="projectName">The project.</param>
        NUnitTask NUnitTask(NunitCmdOptions cmdOptions, params string[] projectName);

        /// <summary>
        /// Task runs nunit tests that are in specified assembly.
        /// </summary>
        /// <param name="testAssemblyFileName"></param>
        /// <returns></returns>
        NUnitTask NUnitTaskByAssemblyName(params string[] testAssemblyFileName);

        /// <summary>
        /// Task runs xunit test that are in specified project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        XunitTask XunitTaskByProjectName(params string[] projectName);

        /// <summary>
        /// Task runs xunit tests that are in specified assembly.
        /// </summary>
        /// <param name="testAssemblyFileName"></param>
        /// <returns></returns>
        XunitTask XunitTaskByAsssemblyName(params string[] testAssemblyFileName);

        /// <summary>
        /// Task will execute tests in the specified <see cref="testAssemblyFileNames"/> list of test assemblies using
        /// the specified NUnit test runner executable.
        /// </summary>
        /// <param name="nunitRunnerFileName">The file path to NUnit's console runner.</param>
        /// <param name="testAssemblyFileNames">The list of of file paths to the assemblies containing unit tests.</param>
        NUnitWithDotCoverTask NUnitWithDotCover(string nunitRunnerFileName, params string[] testAssemblyFileNames);

        /// <summary>
        /// Task  will execute tests in the specified <see cref="testAssemblyFileNames"/> list of test assemblies using
        /// the specified NUnit test runner executable.
        /// </summary>
        /// <param name="nunitRunnerFileName">The file path to NUnit's console runner.</param>
        /// <param name="testAssemblyFileNames">The list of of file paths to the assemblies containing unit tests.</param>
        NUnitWithDotCoverTask NUnitWithDotCover(string nunitRunnerFileName, IList<string> testAssemblyFileNames);

        /// <summary>
        /// Task replaces specified tokens in given file.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        ReplaceTokensTask ReplaceTokensTask(string sourceFileName);

        /// <summary>
        /// Update's specified json.
        /// </summary>
        /// <param name="fileName">File name of the json to be updated.</param>
        /// <returns></returns>
        UpdateJsonFileTask UpdateJsonFileTask(string fileName);

        /// <summary>
        /// Task Fetches build version from file. By default task fetches from file named: {ProductId}.ProjectVersion.Txt
        /// Where ProductId is fetched from <see cref="IBuildPropertiesSession"/> build property named: <see cref="BuildProps.ProductId"/>.
        /// </summary>
        /// <returns></returns>
        FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask();

        /// <summary>
        /// Task fetches build and revision number from external soruce(build system).
        /// Supported build systems by default: AppVeyor, Bamboo, Bitrise, ContinousCl, Jenkins, TFS, TeamCity, TravisCI.
        /// </summary>
        /// <returns></returns>
        FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask();

        /// <summary>
        /// Generate's common assembly info file. Information is taken from <see cref="IBuildPropertiesSession"/>.
        /// </summary>
        /// <returns></returns>
        GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask();

        /// <summary>
        /// Task generate's common assembly info file. Information is taken from <see cref="IBuildPropertiesSession"/>.
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
        /// Task runs open cover tool.
        /// </summary>
        /// <returns></returns>
        OpenCoverTask OpenCoverTask();

        /// <summary>
        /// Task unzip specified zip to specified locattion.
        /// </summary>
        /// <param name="zip">Zip file to be unziped.</param>
        /// <param name="destionation">Destination path where zip content will be unziped.</param>
        /// <returns></returns>
        UnzipTask UnzipTask(string zip, string destionation);

        /// <summary>
        /// Git specific tasks.
        /// </summary>
        /// <returns></returns>
        IGitFluentInterface GitTasks();

        /// <summary>
        /// GitVersion is a tool to help you achieve Semantic Versioning on your project.
        /// https://gitversion.readthedocs.io/en/latest/.
        /// </summary>
        /// <returns></returns>
        GitVersionTask GitVersionTask();

        /// <summary>
        /// Docker specific tasks.
        /// </summary>
        /// <returns></returns>
        DockerFluentInterface DockerTasks();

        /// <summary>
        /// Internet information service specific tasks.
        /// </summary>
        /// <returns></returns>
        IIisTaskFluentInterface IisTasks();

        /// <summary>
        /// Flubu web api specific tasks.
        /// </summary>
        /// <returns></returns>
        IWebApiFluentInterface FlubuWebApiTasks();

        /// <summary>
        /// Task copies file to specified location.
        /// </summary>
        /// <param name="sourceFileName">File to be copied.</param>
        /// <param name="destinationFileName"></param>
        /// <param name="overwrite">If <c>true</c> file is owerwriten if exists. Otherwise not.</param>
        /// <returns></returns>
        CopyFileTask CopyFileTask(string sourceFileName, string destinationFileName, bool overwrite);

        /// <summary>
        /// Task touches (set last write time) specified file.
        /// </summary>
        /// <param name="fileName">File to touch.</param>
        /// <returns></returns>
        TouchFileTask TouchFile(string fileName);

        /// <summary>
        /// Task creates directory.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="forceRecreate">If <c>true</c> directory is deleted if it exists and then created again.</param>
        /// <returns></returns>
        CreateDirectoryTask CreateDirectoryTask(string directoryPath, bool forceRecreate);

        /// <summary>
        /// Task deletes directory.
        /// </summary>
        /// <param name="directoryPath">Path of the directoy to be deleted.</param>
        /// <param name="failIfNotExists">If <c>true</c> task fails if excetpion. Otherwise not.</param>
        /// <returns></returns>
        DeleteDirectoryTask DeleteDirectoryTask(string directoryPath, bool failIfNotExists);

        /// <summary>
        /// Task deletes files in specified directory.
        /// </summary>
        /// <param name="directoryPath">Path of the directoy files to be deleted in.</param>
        /// <param name="filePattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters but doesnt support regular expressions.</param>
        /// <param name="recursive">If <c>true</c> files in subdirectories is searched. Otherwise only in root directory.</param>
        /// <returns></returns>
        DeleteFilesTask DeleteFilesTask(string directoryPath, string filePattern, bool recursive);

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
        /// <param name="tokens">Fist paramteter text to be replaced. Second parameter text to be replaced with.</param>
        /// <returns></returns>
        ReplaceTokensTask ReplaceTextTask(string sourceFile, params Tuple<string, string>[] tokens);

        /// <summary>
        /// Task updates xml. Xml elements can be added, updated or deleted.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        UpdateXmlFileTask UpdateXmlFileTask(string fileName);

        /// <summary>
        /// Sleep for specified period of time.
        /// </summary>
        /// <param name="delay">Delay in milliseconds.</param>
        /// <returns><see cref="SleepTask"/>.</returns>
        SleepTask Sleep(int delay);

        /// <summary>
        /// Sleep for specified period of time.
        /// </summary>
        /// <param name="delay">Delay.</param>
        /// <returns><see cref="SleepTask"/>.</returns>
        SleepTask Sleep(TimeSpan delay);

        /// <summary>
        /// Control windows service with sc.exe command.
        /// </summary>
        /// <param name="command">Command to execute (stop, start, ...)</param>
        /// <returns><see cref="ServiceControlTask"/>.</returns>
        ServiceControlTask ControlService(string command);

        /// <summary>
        /// Control windows service with sc.exe command.
        /// </summary>
        /// <param name="command">Command to execute (stop, start, ...)</param>
        /// <param name="serviceName">Name of the service to control.</param>
        /// <returns><see cref="ServiceControlTask"/>.</returns>
        ServiceControlTask ControlService(string command, string serviceName);

        /// <summary>
        /// Control windows service with sc.exe command.
        /// </summary>
        /// <param name="command">Standard command to execute.</param>
        /// <param name="serviceName">Name of the service to control.</param>
        /// <returns><see cref="ServiceControlTask"/>.</returns>
        ServiceControlTask ControlService(StandardServiceControlCommands command, string serviceName);

        /// <summary>
        /// Creates windows service with sc.exe command.
        /// </summary>
        /// <param name="serviceName">Name of the service to control.</param>
        /// <param name="pathToService">path to service executable .exe.</param>
        /// <returns><see cref="ServiceControlTask"/>.</returns>
        ServiceCreateTask CreateWindowsService(string serviceName, string pathToService);

        /// <summary>
        /// Executes specified powershell script.
        /// </summary>
        /// <param name="pathToPowerShellScript">Path to the power shell script file. Use .\ for relative path.</param>
        /// <returns></returns>
        ExecutePowerShellScriptTask ExecutePowerShellScript(string pathToPowerShellScript);

        /// <summary>
        /// Execute SQL script file with sqlcmd.exe.
        /// </summary>
        /// <returns></returns>
        SqlCmdTask SqlCmdTask(params string[] sqlFiles);

        /// <summary>
        /// Generate T4 template with TextTransform.exe utility.
        /// </summary>
        /// <param name="templateFileName">Filename to T4 transform.</param>
        /// <returns></returns>
        T4TemplateTask GenerateT4Template(string templateFileName);

        /// <summary>
        /// Query status of a windows service with sc.exe command.
        /// </summary>
        /// <param name="serviceName">Name of the service to query.</param>
        /// <returns><see cref="ServiceStatusTask"/>.</returns>
        ServiceStatusTask ServiceStatus(string serviceName);

        /// <summary>
        /// Wait for windows service to stop.
        /// </summary>
        /// <param name="serviceName">Name of the service to query.</param>
        /// <returns><see cref="WaitForServiceStopTask"/>.</returns>
        WaitForServiceStopTask WaitForServiceStop(string serviceName);
    }
}
