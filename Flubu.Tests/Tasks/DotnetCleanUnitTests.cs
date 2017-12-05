using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class DotnetCleanUnitTests : TaskUnitTestBase
    {
        private readonly DotnetCleanTask _task;

        public DotnetCleanUnitTests()
        {
            _task = new DotnetCleanTask();
            _task.DotnetExecutable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceConfigurationTest()
        {
            _task.Project("project");
            _task.Configuration("Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal(3, _task.Arguments.Count);
            Assert.Equal("project", _task.Arguments[0]);
            Assert.Equal("-c", _task.Arguments[1]);
            Assert.Equal("Release", _task.Arguments[2]);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceWithArgumentsTest()
        {
            _task.WithArguments("project");
            _task.WithArguments("--configuration", "Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project", _task.Arguments[0]);
            Assert.Equal("--configuration", _task.Arguments[1]);
            Assert.Equal("Release", _task.Arguments[2]);
        }

        [Fact]
        public void ConfigurationAndProjectFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.SolutionFileName, null)).Returns("project2");
            Properties.Setup(x => x.Get<string>(BuildProps.BuildConfiguration, null)).Returns("Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project2", _task.Arguments[0]);
            Assert.Equal("-c", _task.Arguments[1]);
            Assert.Equal("Release", _task.Arguments[2]);
        }

        [Fact]
        public void ProjectIsFirstArgumentTest()
        {
            _task.WithArguments("-c", "release", "somearg", "-sf");
            _task.Project("project");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("project", _task.Arguments[0]);
        }
    }
}
