using Flubu.Tasks.Process;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class RunProgramTaskTests : TaskTestBase
    {
        [Fact]
        public void ExecuteCommand()
        {
            RunProgramTask task = new RunProgramTask("D:/apps/android/adb.exe");

            task.Execute(Context);
        }
    }
}
