using FlubuCore.Services;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.Nuget;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context
{
    public class TaskFluentInterface
    {
        private readonly TaskContext _context;

        public TaskFluentInterface(TaskContext context)
        {
            _context = context;
        }

        public RunProgramTask RunProgramTask(string programToExecute)
        {
            return _context.CreateTask<RunProgramTask>(programToExecute);
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
            return _context.CreateTask<CopyDirectoryStructureTask>(sourcePath, destinationPath, overwriteExisting);
        }

        public NuGetCmdLineTask NuGetCmdLineTask(string command, string workingDirectory = null)
        {
            return _context.CreateTask<NuGetCmdLineTask>(command, workingDirectory);
        }

        public PublishNuGetPackageTask PublishNuGetPackageTask(string packageId, string nuspecFileName)
        {
            return _context.CreateTask<PublishNuGetPackageTask>(packageId, nuspecFileName);
        }

        public PackageTask PackageTask(string destinationRootDir)
        {
            return _context.CreateTask<PackageTask>(destinationRootDir);
        }

        public CompileSolutionTask CompileSolutionTask(IComponentProvider componentProvider)
        {
            return _context.CreateTask<CompileSolutionTask>(componentProvider);
        }

        public CompileSolutionTask CompileSolutionTask(string solutionFileName, string buildConfiguration, IComponentProvider componentProvider)
        {
            return _context.CreateTask<CompileSolutionTask>(solutionFileName, buildConfiguration, componentProvider);
        }

        public LoadSolutionTask LoadSolutionTask()
        {
            return _context.CreateTask<LoadSolutionTask>();
        }

        public LoadSolutionTask LoadSolutionTask(string solutionFile)
        {
            return _context.CreateTask<LoadSolutionTask>(solutionFile);
        }

        public CoverageReportTask CoverageReportTask(params string[] inputFiles)
        {
            return _context.CreateTask<CoverageReportTask>(inputFiles);
        }

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V3.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public NUnitTask NUnitTaskForNunitV3(string projectName)
        {
            return Tasks.Testing.NUnitTask.ForNunitV3(projectName);
        }

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V2.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public NUnitTask NUnitTaskForNunitV2(string projectName)
        {
            return Tasks.Testing.NUnitTask.ForNunitV2(projectName);
        }

        public NUnitTask NUnitTask(string nunitConsoleFileName, string projectName)
        {
            return _context.CreateTask<NUnitTask>(nunitConsoleFileName, projectName);
        }

        public NUnitTask NUnitTask(string projectName)
        {
            return _context.CreateTask<NUnitTask>(projectName);
        }

        public NUnitTask NUnitTask(
          string testAssemblyFileName,
          string nunitConsoleFileName,
          string workingDirectory)
        {
            return _context.CreateTask<NUnitTask>(testAssemblyFileName, nunitConsoleFileName, workingDirectory);
        }

        public ReplaceTokensTask ReplaceTokensTask(
            string sourceFileName,
            string destinationFileName)
        {
            return _context.CreateTask<ReplaceTokensTask>(sourceFileName, destinationFileName);
        }

        public UpdateJsonFileTask UpdateJsonFileTask(string fileName)
        {
            return _context.CreateTask<UpdateJsonFileTask>(fileName);
        }

        public FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask()
        {
            return _context.CreateTask<FetchBuildVersionFromFileTask>();
        }

        public FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask()
        {
            return _context.CreateTask<FetchVersionFromExternalSourceTask>();
        }

        public GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask()
        {
            return _context.CreateTask<GenerateCommonAssemblyInfoTask>();
        }
    }
}
