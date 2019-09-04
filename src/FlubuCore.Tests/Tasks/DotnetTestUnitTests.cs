using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class DotnetTestUnitTests : TaskUnitTestBase
    {
        private readonly DotnetTestTask _task;

        public DotnetTestUnitTests()
        {
            _task = new DotnetTestTask();
            _task.Executable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceConfigurationTest()
        {
            _task.Project("project");
            _task.Configuration("Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal(3, _task.GetArguments().Count);
            Assert.Equal("project", _task.GetArguments()[0]);
            Assert.Equal("--configuration", _task.GetArguments()[1]);
            Assert.Equal("Release", _task.GetArguments()[2]);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceWithArgumentsTest()
        {
            _task.WithArguments("project");
            _task.WithArguments("--configuration", "Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project", _task.GetArguments()[0]);
            Assert.Equal("--configuration", _task.GetArguments()[1]);
            Assert.Equal("Release", _task.GetArguments()[2]);
        }

        [Fact]
        public void ConfigurationAndProjectFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.SolutionFileName, null, It.IsAny<string>())).Returns("project2");
            Properties.Setup(x => x.Get<string>(BuildProps.BuildConfiguration, null, "BeforeExecute")).Returns("Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project2", _task.GetArguments()[0]);
            Assert.Equal("--configuration", _task.GetArguments()[1]);
            Assert.Equal("Release", _task.GetArguments()[2]);
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
