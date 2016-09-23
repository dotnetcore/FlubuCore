using Flubu.Tasks.Dotnet;
using Microsoft.DotNet.Cli.Utils;
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

        [Fact(Skip = "Implement task first")]
        public void ExecuteNonExistentCommand()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("nonexist");

            Assert.Throws<CommandUnknownException>(() => task.Execute(Context));
        }
    }
}
