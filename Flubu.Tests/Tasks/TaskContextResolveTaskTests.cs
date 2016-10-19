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
    }
}
