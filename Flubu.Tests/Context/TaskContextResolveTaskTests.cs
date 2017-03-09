using System;
using FlubuCore.Tasks.NetCore;
using Xunit;

namespace Flubu.Tests.Context
{
    using FlubuCore.Tasks.Iis;

    [Collection(nameof(FlubuTestCollection))]
    public class TaskContextResolveTaskTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public TaskContextResolveTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ResolveRunProgramTaskTest()
        {
            Context.Tasks().RunProgramTask("test");
        }

        [Fact]
        public void ResolveDirectoryStructureTaskTest()
        {
            Context.Tasks().CopyDirectoryStructureTask("test", "test2", true);
        }

        [Fact]
        public void ResolveExecuteDotNetTaskTest()
        {
            Context.CoreTasks().ExecuteDotnetTask("command");
        }

        [Fact]
        public void ResolveExecuteDotNetTask2Test()
        {
            Context.CoreTasks().ExecuteDotnetTask(StandardDotnetCommands.Publish);
        }

        [Fact]
        public void ResloveCompileSolutionTaskTest()
        {
            Context.Tasks().CompileSolutionTask();
        }

        [Fact]
        public void ResloveCompileSolutionTask2Test()
        {
            Context.Tasks().CompileSolutionTask("sln", "release");
        }

        [Fact]
        public void ResolvePublishNugetPackageTaskTest()
        {
            Context.Tasks().PublishNuGetPackageTask("packageId", "nuspec");
        }

        [Fact]
        public void ResolveNunitTaskTest()
        {
            Context.Tasks().NUnitTaskByAssemblyName("pn");
        }

        [Fact]
        public void ResolveNunitTask2Test2()
        {
            Context.Tasks().NUnitTaskByProjectName("pn", "tt");
        }

        [Fact]
        public void ResolveNunitTaskTest3()
        {
            Context.Tasks().NUnitTaskByAssemblyName("pn", "tt", "xx");
        }

        [Fact]
        public void ResolveNunitTaskForNunitV3Test()
        {
            Context.Tasks().NUnitTaskForNunitV3("test");
        }

        [Fact]
        public void ResolveNunitTaskForNunitV2Test()
        {
            Context.Tasks().NUnitTaskForNunitV2("test");
        }

        [Fact]
        public void ResolveCoverageReportTaskTest()
        {
            Context.Tasks().CoverageReportTask("a", "b");
        }

        [Fact]
        public void ResolveLoadSolutionTaskTest()
        {
            Context.Tasks().LoadSolutionTask();
        }

        [Fact]
        public void ResolveLoadSolutionTaskTest2()
        {
            Context.Tasks().LoadSolutionTask("test");
        }

        [Fact]
        public void ResolveFetchBuildVersionFromFileTest()
        {
            Context.Tasks().FetchBuildVersionFromFileTask();
        }

        [Fact]
        public void ResolveFetchVersionFromExternalSourceTaskTest()
        {
            Context.Tasks().FetchVersionFromExternalSourceTask();
        }

        [Fact]
        public void ResolveGenerateCommonAssemblyInfoTaskTest()
        {
            Context.Tasks().GenerateCommonAssemblyInfoTask();
        }

        [Fact]
        public void ResolveGenerateCommonAssemblyInfoTaskTest2()
        {
            Context.Tasks().GenerateCommonAssemblyInfoTask(new Version(1, 0, 0, 0));
        }

        [Fact]
        public void ResolveReplaceTokensTaskTest()
        {
            Context.Tasks().ReplaceTokensTask("a", "b");
        }

        [Fact]
        public void ResolveUpdateJsonFileTaskTest()
        {
            Context.Tasks().UpdateJsonFileTask("a");
        }

        [Fact]
        public void ResolveCreateWebsiteTaskTaskTest()
        {
            Context.Tasks().IisTasks().CreateWebsiteTask();
        }

        [Fact]
        public void ResolveCreateWebApplicationTaskTest()
        {
            Context.Tasks().IisTasks().CreateWebApplicationTask("test");
        }

        [Fact]
        public void ResolveCreateAppPoolTaskTest()
        {
            Context.Tasks().IisTasks().CreateAppPoolTask("test");
        }

        [Fact]
        public void ResolveAddWebsiteBindingTaskTest()
        {
            Context.Tasks().IisTasks().AddWebsiteBindingTask();
        }

        [Fact]
        public void ResolveDeleteAppPoolTaskTest()
        {
            Context.Tasks().IisTasks().DeleteAppPoolTask("test");
        }

        [Fact]
        public void ResolveControlAppPoolTaskTest()
        {
            Context.Tasks().IisTasks().ControlAppPoolTask("test", ControlApplicationPoolAction.Start);
        }

        [Fact]
        public void ResolveUpdateNetCoreVersionTaskTest()
        {
            Context.CoreTasks().UpdateNetCoreVersionTask("test", "test2");
        }

        [Fact]
        public void ResolveCopyFileTask()
        {
            Context.Tasks().CopyFileTask("a", "b", true);
        }

        [Fact]
        public void ResolveCreateDirectoryTask()
        {
            Context.Tasks().CreateDirectoryTask("a", true);
        }

        [Fact]
        public void ResolveDeleteDirectoryTask()
        {
            Context.Tasks().DeleteDirectoryTask("a", true);
        }

        [Fact]
        public void ResolveDeleteFilesTask()
        {
            Context.Tasks().DeleteFilesTask("a", "b", false);
        }

        [Fact]
        public void ResolveUnzipFileTask()
        {
            Context.Tasks().UnzipTask("t", "d");
        }

        [Fact]
        public void ResolveOpenCoverTask()
        {
            Context.Tasks().OpenCoverTask();
        }

        [Fact]
        public void ResolveOpenCoverToCoberturaTask()
        {
            Context.Tasks().OpenCoverToCoberturaTask("in", "out");
        }

        [Fact]
        public void ResolveLinuxSystemCtlTask()
        {
          Assert.NotNull(Context.CoreTasks().LinuxTasks().SystemCtlTask("cmd", "srvice"));
        }

        [Fact]
        public void ResolvePackageTask()
        {
            Context.Tasks().PackageTask(string.Empty);
        }

        [Fact]
        public void ResolvePackageTaskWithNull()
        {
            Assert.Throws<ArgumentNullException>(() => Context.Tasks().PackageTask(null));
        }

        [Fact]
        public void ResolveCleanOutputTask()
        {
            Context.Tasks().CleanOutputTask();
        }

        [Fact]
        public void ResolveUpdateXmlFileTask()
        {
            Context.Tasks().UpdateXmlFileTask("test");
        }

        [Fact]
        public void ResolveDotnetRestoreTask()
        {
            Context.CoreTasks().Restore();
        }

        [Fact]
        public void ResolveDotnetBuildTask()
        {
            Context.CoreTasks().Build();
        }

        [Fact]
        public void ResolveDotnetPublishTask()
        {
            Context.CoreTasks().Publish();
        }

        [Fact]
        public void ResolveDotnetTestTask()
        {
            Context.CoreTasks().Test();
        }

        [Fact]
        public void ResolveDotnetPackTask()
        {
            Context.CoreTasks().Pack();
        }

        [Fact]
        public void ResolveDotnetCleanTask()
        {
            Context.CoreTasks().Clean();
        }
    }
}
