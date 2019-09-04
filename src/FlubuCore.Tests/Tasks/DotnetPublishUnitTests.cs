using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class DotnetPublishUnitTests : TaskUnitTestBase
    {
        private readonly DotnetPublishTask _task;

        public DotnetPublishUnitTests()
        {
            _task = new DotnetPublishTask();
            _task.Executable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.BuildConfiguration, null, It.IsAny<string>())).Returns("Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("--configuration", _task.GetArguments()[0]);
            Assert.Equal("Release", _task.GetArguments()[1]);
        }

        [Fact]
        public void ConfigurationFromFluentInterfaceConfigurationTest()
        {
            _task.Configuration("Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal(2, _task.GetArguments().Count);
            Assert.Equal("--configuration", _task.GetArguments()[0]);
            Assert.Equal("Release", _task.GetArguments()[1]);
        }

        [Fact]
        public void ConfigurationFromFluentInterfaceWithArgumentsTest()
        {
            _task.WithArguments("--configuration", "Release");
            _task.ExecuteVoid(Context.Object);
            Assert.Equal("--configuration", _task.GetArguments()[0]);
            Assert.Equal("Release", _task.GetArguments()[1]);
        }
    }
}