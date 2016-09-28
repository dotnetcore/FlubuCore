using FlubuCore.Tasks.Process;
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
            RunProgramTask task = new RunProgramTask("C:/Program Files/dotnet/dotnet.exe");

            task
                .WithArguments("--version")
                .Execute(Context);
        }
    }
}
