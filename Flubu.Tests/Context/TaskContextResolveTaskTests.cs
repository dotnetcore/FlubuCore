using FlubuCore.Tasks.NetCore;
using Xunit;

namespace Flubu.Tests.Context
{
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

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveNunitTaskTest()
        {
            Context.Tasks().NUnitTask("pn");
        }

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveNunitTas2Test2()
        {
            Context.Tasks().NUnitTask("pn", "tt");
        }

        [Fact]
        public void ResolveNunitTaskTest3()
        {
            Context.Tasks().NUnitTask("pn", "tt", "xx");
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

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveCoverageReportTaskTest()
        {
            Context.Tasks().CoverageReportTask("a", "b");
        }

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveLoadSolutionTaskTest()
        {
            Context.Tasks().LoadSolutionTask();
        }

        [Fact]
        public void ResolveLoadSolutionTaskTest2()
        {
            Context.Tasks().LoadSolutionTask("test");
        }

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveFetchBuildVersionFromFileTest()
        {
            Context.Tasks().FetchBuildVersionFromFileTask();
        }

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveFetchVersionFromExternalSourceTaskTest()
        {
            Context.Tasks().FetchVersionFromExternalSourceTask();
        }

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveGenerateCommonAssemblyInfoTaskTest()
        {
            Context.Tasks().GenerateCommonAssemblyInfoTask();
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

        [Fact(Skip = "Task Factory doesnt work correctly.")]
        public void ResolveUpdateNetCoreVersionTaskTest()
        {
            Context.CoreTasks().UpdateNetCoreVersionTask("test");
        }
    }
}
