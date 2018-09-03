using System.Runtime.InteropServices;
using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class ExecuteDotnetTaskTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public ExecuteDotnetTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ExecuteNonExistentCommand()
        {
            ExecuteDotnetTask task = Context.CoreTasks().ExecuteDotnetTask("nonexist")
                .DotnetExecutable(PathToDotnetExecutable);

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));
        }

        [Fact]
        public void ExecuteWrongArgsCommand()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("build")
                .DotnetExecutable(PathToDotnetExecutable)
                .WithArguments("Flubu.NonExtstProj");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.Execute(Context));

            Assert.Equal(1, e.ErrorCode);
        }

        [Fact]
        public void ExecuteCommandDotnetNotSet()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("help");

            var e = Assert.Throws<TaskExecutionException>(() => task.Execute(Context));
            Assert.Equal(-1, e.ErrorCode);
        }

        [Fact]
        public void ExecuteCommand()
        {
            ExecuteDotnetTask task = Context.CoreTasks().ExecuteDotnetTask("help")
                .DotnetExecutable(PathToDotnetExecutable);

            var res = task.Execute(Context);

            Assert.Equal(0, res);
        }

        [Fact]
        public void ExecuteCommandTargetTreeCreate()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("help");
            task.DotnetExecutable(PathToDotnetExecutable);

            int res = task.Execute(Context);
            Assert.Equal(0, res);
        }
    }
}
