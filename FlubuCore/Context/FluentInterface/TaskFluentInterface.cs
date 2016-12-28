using System;
using System.Linq;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.Nuget;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface
{
    public class TaskFluentInterface : ITaskFluentInterface
    {
        private readonly IisTaskFluentInterface _iisTasksFluentInterface;

        public TaskFluentInterface(IIisTaskFluentInterface iisTasksFluentInterface)
        {
            _iisTasksFluentInterface = (IisTaskFluentInterface)iisTasksFluentInterface;
        }

        public TaskContext Context { get; set; }

        public IRunProgramTask RunProgramTask(string programToExecute)
        {
            return Context.CreateTask<RunProgramTask>(programToExecute);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CopyDirectoryStructureTask" /> class
        ///     using a specified source and destination path and an indicator whether to overwrite existing files.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwriteExisting">if set to <c>true</c> the task will overwrite existing destination files.</param>
        public CopyDirectoryStructureTask CopyDirectoryStructureTask(string sourcePath, string destinationPath, bool overwriteExisting)
        {
            return Context.CreateTask<CopyDirectoryStructureTask>(sourcePath, destinationPath, overwriteExisting);
        }

        public NuGetCmdLineTask NuGetCmdLineTask(string command, string workingDirectory = null)
        {
            return Context.CreateTask<NuGetCmdLineTask>(command, workingDirectory);
        }

        public PublishNuGetPackageTask PublishNuGetPackageTask(string packageId, string nuspecFileName)
        {
            return Context.CreateTask<PublishNuGetPackageTask>(packageId, nuspecFileName);
        }

        public PackageTask PackageTask(string destinationRootDir)
        {
            if (destinationRootDir == null)
                throw new ArgumentNullException(nameof(destinationRootDir));

            return Context.CreateTask<PackageTask>(destinationRootDir);
        }

        public CompileSolutionTask CompileSolutionTask()
        {
            return Context.CreateTask<CompileSolutionTask>();
        }

        public CompileSolutionTask CompileSolutionTask(string solutionFileName, string buildConfiguration)
        {
            return Context.CreateTask<CompileSolutionTask>(solutionFileName, buildConfiguration);
        }

        public LoadSolutionTask LoadSolutionTask()
        {
            return Context.CreateTask<LoadSolutionTask>();
        }

        public LoadSolutionTask LoadSolutionTask(string solutionFileName)
        {
            return Context.CreateTask<LoadSolutionTask>(solutionFileName);
        }

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V3.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public NUnitTask NUnitTaskForNunitV3(params string[] projectName)
        {
            return Tasks.Testing.NUnitTask.ForNunitV3(projectName);
        }

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V2.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public NUnitTask NUnitTaskForNunitV2(params string[] projectName)
        {
            return Tasks.Testing.NUnitTask.ForNunitV2(projectName);
        }

        public NUnitTask NUnitTaskByProjectName(params string[] projectName)
        {
            return Context.CreateTask<NUnitTask>(projectName.ToList());
        }

        public NUnitTask NUnitTaskByAssemblyName(params string[] testAssemblyFileName)
        {
            var task = new NUnitTask();
            task.TestAssemblyFileNames = testAssemblyFileName.ToList();

            return task;
        }

        public ReplaceTokensTask ReplaceTokensTask(
            string sourceFileName,
            string destinationFileName)
        {
            return Context.CreateTask<ReplaceTokensTask>(sourceFileName, destinationFileName);
        }

        public UpdateJsonFileTask UpdateJsonFileTask(string fileName)
        {
            return Context.CreateTask<UpdateJsonFileTask>(fileName);
        }

        public FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask()
        {
            return Context.CreateTask<FetchBuildVersionFromFileTask>();
        }

        public FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask()
        {
            return Context.CreateTask<FetchVersionFromExternalSourceTask>();
        }

        public GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask()
        {
            return Context.CreateTask<GenerateCommonAssemblyInfoTask>();
        }

        public GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask(Version buildVersion)
        {
            return Context.CreateTask<GenerateCommonAssemblyInfoTask>(buildVersion);
        }

        public CoverageReportTask CoverageReportTask(params string[] inputFiles)
        {
            return new CoverageReportTask(inputFiles);
        }

        public OpenCoverToCoberturaTask OpenCoverToCoberturaTask(string input, string output)
        {
            return Context.CreateTask<OpenCoverToCoberturaTask>(input, output);
        }

        public OpenCoverTask OpenCoverTask()
        {
            return Context.CreateTask<OpenCoverTask>();
        }

        public UnzipTask UnzipTask(string zip, string destination)
        {
            return Context.CreateTask<UnzipTask>(zip, destination);
        }

        public IIisTaskFluentInterface IisTasks()
        {
            _iisTasksFluentInterface.Context = Context;
            return _iisTasksFluentInterface;
        }

        public CopyFileTask CopyFileTask(string sourceFileName, string destinationFileName, bool overwrite)
        {
            return Context.CreateTask<CopyFileTask>(sourceFileName, destinationFileName, overwrite);
        }

        public CreateDirectoryTask CreateDirectoryTask(string directoryPath, bool forceRecreate)
        {
            return Context.CreateTask<CreateDirectoryTask>(directoryPath, forceRecreate);
        }

        public DeleteDirectoryTask DeleteDirectoryTask(string directoryPath, bool failIfNotExists)
        {
            return Context.CreateTask<DeleteDirectoryTask>(directoryPath, failIfNotExists);
        }

        public DeleteFilesTask DeleteFilesTask(string directoryPath, string filePattern, bool recursive)
        {
            return Context.CreateTask<DeleteFilesTask>(directoryPath, filePattern, recursive);
        }

        public MergeConfigurationTask MergeConfigurationTask(string outFile)
        {
            return Context.CreateTask<MergeConfigurationTask>(outFile);
        }

        public MergeConfigurationTask MergeConfigurationTask(string outFile, params string[] sourceFiles)
        {
            return Context.CreateTask<MergeConfigurationTask>(outFile, sourceFiles);
        }

        public ReplaceTextTask ReplaceTextTask(string sourceFile, params Tuple<string, string>[] tokens)
        {
            return Context.CreateTask<ReplaceTextTask>(sourceFile).Replace(tokens);
        }

        public CleanOutputTask CleanOutputTask()
        {
            return Context.CreateTask<CleanOutputTask>();
        }
    }
}
