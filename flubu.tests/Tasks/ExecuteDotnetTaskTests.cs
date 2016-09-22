using Flubu.Tasks.Dotnet;
using Microsoft.DotNet.Cli.Utils;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class ExecuteDotnetTaskTests : TaskTestBase
    {
        [Fact]
        public void ExecuteNonExistentCommand()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("nonexist");

            Assert.Throws<CommandUnknownException>(() => task.Execute(Context.Object));
        }
    }
}
