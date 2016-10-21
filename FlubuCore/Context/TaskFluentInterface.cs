using System;
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
    public class TaskFluentInterface : ITaskFluentInterface
    {
        public TaskContext Context { get; set; }

        public RunProgramTask RunProgramTask(string programToExecute)
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

        public LoadSolutionTask LoadSolutionTask(string solutionFile)
        {
            return Context.CreateTask<LoadSolutionTask>(solutionFile);
        }

        public CoverageReportTask CoverageReportTask(params string[] inputFiles)
        {
            return Context.CreateTask<CoverageReportTask>(inputFiles);
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

        public NUnitTask NUnitTask(string projectName, string nunitConsoleFileName = null)
        {
            return Context.CreateTask<NUnitTask>(projectName);
        }

        public NUnitTask NUnitTask(
          string testAssemblyFileName,
          string nunitConsoleFileName,
          string workingDirectory)
        {
            throw new NotSupportedException("Not supported for now.");
            ////return _context.CreateTask<NUnitTask>(testAssemblyFileName, nunitConsoleFileName, workingDirectory);
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
    }
}
