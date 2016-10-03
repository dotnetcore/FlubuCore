using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(TaskTestCollection))]
    public class ExecuteDotnetTaskTests : TaskTestBase
    {
        private readonly TaskTestFixture _fixture;

        public ExecuteDotnetTaskTests(TaskTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ExecuteNonExistentCommand()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("nonexist");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.Execute(Context));
        }

        [Fact]
        public void ExecuteWrongArgsCommand()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("build")
                .WithArguments("Flubu.NonExtstProj");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.Execute(Context));

            Assert.Equal(-1, e.ErrorCode);
        }
    }
}
