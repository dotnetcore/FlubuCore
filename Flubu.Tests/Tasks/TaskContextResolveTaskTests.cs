using FlubuCore.Tasks.NetCore;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(TaskTestCollection))]
    public class TaskContextResolveTaskTests : TaskTestBase
    {
        private readonly TaskTestFixture _fixture;

        public TaskContextResolveTaskTests(TaskTestFixture fixture)
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
    }
}
