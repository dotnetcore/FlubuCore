using Flubu.Tasks.Process;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(TaskTestCollection))]
    public class RunProgramTaskTests : TaskTestBase
    {
        private readonly TaskTestFixture _fixture;

        public RunProgramTaskTests(TaskTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ExecuteCommand()
        {
            RunProgramTask task = new RunProgramTask("D:/apps/android/adb.exe");

            task.Execute(Context);
        }
    }
}
