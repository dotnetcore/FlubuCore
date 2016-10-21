using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public interface ITaskFluentInterface
    {
        TaskContext Context { get; set; }

        RunProgramTask RunProgramTask(string programToExecute);

        CopyDirectoryStructureTask CopyDirectoryStructureTask(string sourcePath, string destinationPath, bool overwriteExisting);

        NuGetCmdLineTask NuGetCmdLineTask(string command, string workingDirectory = null);

        PublishNuGetPackageTask PublishNuGetPackageTask(string packageId, string nuspecFileName);

        PackageTask PackageTask(string destinationRootDir);

        CompileSolutionTask CompileSolutionTask();

        CompileSolutionTask CompileSolutionTask(string solutionFileName, string buildConfiguration);

        LoadSolutionTask LoadSolutionTask();

        LoadSolutionTask LoadSolutionTask(string solutionFile);

        CoverageReportTask CoverageReportTask(params string[] inputFiles);

        NUnitTask NUnitTaskForNunitV3(string projectName);

        NUnitTask NUnitTaskForNunitV2(string projectName);

        NUnitTask NUnitTask(string projectName, string nunitConsoleFileName = null);

        NUnitTask NUnitTask(string testAssemblyFileName, string nunitConsoleFileName, string workingDirectory);

        ReplaceTokensTask ReplaceTokensTask(string sourceFileName, string destinationFileName);

        UpdateJsonFileTask UpdateJsonFileTask(string fileName);

        FetchBuildVersionFromFileTask FetchBuildVersionFromFileTask();

        FetchVersionFromExternalSourceTask FetchVersionFromExternalSourceTask();

        GenerateCommonAssemblyInfoTask GenerateCommonAssemblyInfoTask();
    }
}
