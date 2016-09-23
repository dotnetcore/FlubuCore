using Flubu.Tasks.Dotnet;
using Microsoft.DotNet.Cli.Utils;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class ExecuteDotnetTaskTests : TaskTestBase
    {
        [Fact(Skip = "Implement task first")]
        public void ExecuteNonExistentCommand()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("nonexist");

            Assert.Throws<CommandUnknownException>(() => task.Execute(Context));
        }
    }
}
