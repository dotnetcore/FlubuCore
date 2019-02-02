using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Tasks.FileSystem;
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

namespace FlubuCore.Context.FluentInterface
{
    /// <inheritdoc />
    public class TaskFluentInterface : ITaskFluentInterface
    {
        private readonly IisTaskFluentInterface _iisTasksFluentInterface;

        private readonly WebApiFluentInterface _webApiFluentInterface;

        private readonly GitFluentInterface _gitFluentInterface;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly DockerFluentInterface _dockerFluentInterface;

        /// <inheritdoc />
        public TaskFluentInterface(IIisTaskFluentInterface iisTasksFluentInterface, IWebApiFluentInterface webApiFluentInterface, IGitFluentInterface gitFluentInterface, DockerFluentInterface dockerFluentInterface, IHttpClientFactory httpClientFactory)
        {
            _iisTasksFluentInterface = (IisTaskFluentInterface)iisTasksFluentInterface;
            _webApiFluentInterface = (WebApiFluentInterface)webApiFluentInterface;
            _gitFluentInterface = (GitFluentInterface)gitFluentInterface;
            _dockerFluentInterface = (DockerFluentInterface)dockerFluentInterface;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get or set <see cref="TaskContext"/> for fluent interface.
        /// </summary>
        public TaskContext Context { get; set; }

        /// <inheritdoc />
        public IRunProgramTask RunProgramTask(string programToExecute)
        {
            return Context.CreateTask<RunProgramTask>(programToExecute);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="M:FlubuCore.Context.FluentInterface.TaskFluentInterface.CopyDirectoryStructureTask(System.String,System.String,System.Boolean)" /> class
        ///     using a specified source and destination path and an indicator whether to overwrite existing files.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwriteExisting">if set to <c>true</c> the task will overwrite existing destination files.</param>
        public CopyDirectoryStructureTask CopyDirectoryStructureTask(string sourcePath, string destinationPath, bool overwriteExisting)
        {
            return Context.CreateTask<CopyDirectoryStructureTask>(sourcePath, destinationPath, overwriteExisting);
        }

        public HttpClient CreateHttpClient(string baseUrl)
        {
          return _httpClientFactory.Create(baseUrl);
        }

        /// <inheritdoc />
        public NuGetCmdLineTask NuGetCmdLineTask(string command, string workingDirectory = null)
        {
            return Context.CreateTask<NuGetCmdLineTask>(command, workingDirectory);
        }

        /// <inheritdoc />
        public PublishNuGetPackageTask PublishNuGetPackageTask(string packageId, string nuspecFileName)
        {
            return Context.CreateTask<PublishNuGetPackageTask>(packageId, nuspecFileName);
        }

        /// <inheritdoc />
        public PackageTask PackageTask(string destinationRootDir)
        {
            if (destinationRootDir == null)
                throw new ArgumentNullException(nameof(destinationRootDir));

            return Context.CreateTask<PackageTask>(destinationRootDir);
        }

        /// <inheritdoc />
        public CompileSolutionTask CompileSolutionTask()
        {
            return Context.CreateTask<CompileSolutionTask>();
        }

        /// <inheritdoc />
        public CompileSolutionTask CompileSolutionTask(string solutionFileName, string buildConfiguration)
        {
            return Context.CreateTask<CompileSolutionTask>(solutionFileName, buildConfiguration);
        }

        /// <inheritdoc />
        public LoadSolutionTask LoadSolutionTask()
        {
            return Context.CreateTask<LoadSolutionTask>();
        }

        /// <inheritdoc />
        public LoadSolutionTask LoadSolutionTask(string solutionFileName)
        {
            return Context.CreateTask<LoadSolutionTask>(solutionFileName);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V3.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public NUnitTask NUnitTaskForNunitV3(params string[] projectName)
        {
            return NUnitTask.ForNunitV3(projectName);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V2.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public NUnitTask NUnitTaskForNunitV2(params string[] projectName)
        {
            return NUnitTask.ForNunitV2(projectName);
        }

        /// <inheritdoc />
        public NUnitTask NUnitTaskByProjectName(params string[] projectName)
        {
            return Context.CreateTask<NUnitTask>(projectName.ToList());
        }

        /// <inheritdoc />
        public NUnitTask NUnitTaskByAssemblyName(params string[] testAssemblyFileName)
        {
            var task = new NUnitTask();
            task.TestAssemblyFileNames.AddRange(testAssemblyFileName);
            return task;
        }

        public XunitTask XunitTaskByProjectName(params string[] projectName)
        {
            var task = new XunitTask(projectName.ToList());
            return task;
        }

        public XunitTask XunitTaskByAsssemblyName(params string[] testAssemblyFileName)
        {
            var task = new XunitTask(assemblyNames: testAssemblyFileName.ToList());
            return task;
        }

        /// <inheritdoc />
        public NUnitWithDotCoverTask NUnitWithDotCover(string nunitRunnerFileName, params string[] testAssemblyFileNames)
        {
            return Context.CreateTask<NUnitWithDotCoverTask>(nunitRunnerFileName, testAssemblyFileNames);
        }

        /// <inheritdoc />
        public NUnitWithDotCoverTask NUnitWithDotCover(string nunitRunnerFileName, IList<string> testAssemblyFileNames)
        {
            return Context.CreateTask<NUnitWithDotCoverTask>(nunitRunnerFileName, testAssemblyFileNames.ToArray());
        }

        /// <inheritdoc />
        public ReplaceTokensTask ReplaceTokensTask(string sourceFileName)
        {
            return Context.CreateTask<ReplaceTokensTask>(sourceFileName);
        }

        /// <inheritdoc />
        public UpdateJsonFileTask UpdateJsonFileTask(string fileName)
        {
            return Context.CreateTask<UpdateJsonFileTask>(fileName);
        }

        /// <inheritdoc />
        public FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask()
        {
            return Context.CreateTask<FetchBuildVersionFromFileTask>();
        }

        /// <inheritdoc />
        public FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask()
        {
            return Context.CreateTask<FetchVersionFromExternalSourceTask>();
        }

        /// <inheritdoc />
        public GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask()
        {
            return Context.CreateTask<GenerateCommonAssemblyInfoTask>();
        }

        /// <inheritdoc />
        public GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask(Version buildVersion)
        {
            return Context.CreateTask<GenerateCommonAssemblyInfoTask>(buildVersion);
        }

        /// <inheritdoc />
        public CoverageReportTask CoverageReportTask(params string[] inputFiles)
        {
            return new CoverageReportTask(inputFiles);
        }

        /// <inheritdoc />
        public OpenCoverToCoberturaTask OpenCoverToCoberturaTask(string input, string output)
        {
            return Context.CreateTask<OpenCoverToCoberturaTask>(input, output);
        }

        /// <inheritdoc />
        public OpenCoverTask OpenCoverTask()
        {
            return Context.CreateTask<OpenCoverTask>();
        }

        /// <inheritdoc />
        public UnzipTask UnzipTask(string zip, string destination)
        {
            return Context.CreateTask<UnzipTask>(zip, destination);
        }

        public IGitFluentInterface GitTasks()
        {
            _gitFluentInterface.Context = Context;
            return _gitFluentInterface;
        }

        public GitVersionTask GitVersionTask()
        {
            return Context.CreateTask<GitVersionTask>();
        }

        public DockerFluentInterface DockerTasks()
        {
            return _dockerFluentInterface;
        }

        /// <inheritdoc />
        public IIisTaskFluentInterface IisTasks()
        {
            _iisTasksFluentInterface.Context = Context;
            return _iisTasksFluentInterface;
        }

        /// <inheritdoc />
        public IWebApiFluentInterface FlubuWebApiTasks()
        {
            _webApiFluentInterface.Context = Context;
            return _webApiFluentInterface;
        }

        /// <inheritdoc />
        public CopyFileTask CopyFileTask(string sourceFileName, string destinationFileName, bool overwrite)
        {
            return Context.CreateTask<CopyFileTask>(sourceFileName, destinationFileName, overwrite);
        }

        /// <inheritdoc />
        public TouchFileTask TouchFile(string fileName)
        {
            return Context.CreateTask<TouchFileTask>(fileName);
        }

        /// <inheritdoc />
        public CreateDirectoryTask CreateDirectoryTask(string directoryPath, bool forceRecreate)
        {
            return Context.CreateTask<CreateDirectoryTask>(directoryPath, forceRecreate);
        }

        /// <inheritdoc />
        public DeleteDirectoryTask DeleteDirectoryTask(string directoryPath, bool failIfNotExists)
        {
            return Context.CreateTask<DeleteDirectoryTask>(directoryPath, failIfNotExists);
        }

        /// <inheritdoc />
        public DeleteFilesTask DeleteFilesTask(string directoryPath, string filePattern, bool recursive)
        {
            return Context.CreateTask<DeleteFilesTask>(directoryPath, filePattern, recursive);
        }

        /// <inheritdoc />
        public MergeConfigurationTask MergeConfigurationTask(string outFile, params string[] sourceFiles)
        {
            return Context.CreateTask<MergeConfigurationTask>(outFile, sourceFiles.ToList());
        }

        /// <inheritdoc />
        public ReplaceTokensTask ReplaceTextTask(string sourceFile, params Tuple<string, string>[] tokens)
        {
            return Context.CreateTask<ReplaceTokensTask>(sourceFile).Replace(tokens);
        }

        /// <inheritdoc />
        public CleanOutputTask CleanOutputTask()
        {
            return Context.CreateTask<CleanOutputTask>();
        }

        /// <inheritdoc />
        public UpdateXmlFileTask UpdateXmlFileTask(string fileName)
        {
            return Context.CreateTask<UpdateXmlFileTask>(fileName);
        }

        /// <inheritdoc />
        public SleepTask Sleep(int delay)
        {
            return Context.CreateTask<SleepTask>(delay);
        }

        /// <inheritdoc />
        public SleepTask Sleep(TimeSpan delay)
        {
            return Context.CreateTask<SleepTask>(delay);
        }

        public ServiceControlTask ControlService(string command)
        {
            return Context.CreateTask<ServiceControlTask>(command, string.Empty);
        }

        /// <inheritdoc />
        public ServiceControlTask ControlService(string command, string serviceName)
        {
            return Context.CreateTask<ServiceControlTask>(command, serviceName);
        }

        public ServiceControlTask ControlService(StandardServiceControlCommands command, string serviceName)
        {
            return Context.CreateTask<ServiceControlTask>(command.ToString(), serviceName);
        }

        public ServiceCreateTask CreateWindowsService(string serviceName, string pathToService)
        {
            return Context.CreateTask<ServiceCreateTask>(serviceName, pathToService);
        }

        public ExecutePowerShellScriptTask ExecutePowerShellScript(string pathToPowerShellScript)
        {
            return Context.CreateTask<ExecutePowerShellScriptTask>(pathToPowerShellScript);
        }

        public SqlCmdTask SqlCmdTask(params string[] sqlFiles)
        {
            return Context.CreateTask<SqlCmdTask>(sqlFiles.ToList());
        }

        public T4TemplateTask GenerateT4Template(string templateFileName)
        {
            return Context.CreateTask<T4TemplateTask>(templateFileName);
        }
    }
}
