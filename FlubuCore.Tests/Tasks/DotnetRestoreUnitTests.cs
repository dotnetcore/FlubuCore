using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class DotnetRestoreUnitTests : TaskUnitTestBase
    {
        private readonly DotnetRestoreTask _task;

        public DotnetRestoreUnitTests()
        {
            _task = new DotnetRestoreTask();
            _task.Executable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceConfigurationTest()
        {
            _task.Project("project");
            _task.ExecuteVoid(Context.Object);
            Assert.Single(_task.GetArguments());
            Assert.Equal("project", _task.GetArguments()[0]);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceWithArgumentsTest()
        {
            _task.WithArguments("project");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project", _task.GetArguments()[0]);
        }

        [Fact]
        public void ConfigurationAndProjectFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.SolutionFileName, null,  It.IsAny<string>())).Returns("project2");
            _task.ExecuteVoid(Context.Object);
            Assert.Single(_task.GetArguments());
            Assert.Equal("project2", _task.GetArguments()[0]);
        }

        [Fact]
        public void ProjectIsFirstArgumentTest()
        {
            _task.WithArguments("-c", "release", "somearg", "-sf");
            _task.Project("project");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project", _task.GetArguments()[0]);
        }
    }
}
