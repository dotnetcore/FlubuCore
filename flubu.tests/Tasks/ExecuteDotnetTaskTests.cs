using Flubu.Tasks.Dotnet;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class ExecuteDotnetTaskTests : TaskTestBase
    {
        [Fact]
        public void SimpleExecute()
        {
            ExecuteDotnetTask task = new ExecuteDotnetTask("test");

            task.Execute(Context.Object);
        }
    }
}
