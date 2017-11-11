using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Process;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
  
    public class DotnetPackUnitTests : TaskUnitTestBase
    {
        private DotnetPackTask task;
        
        public DotnetPackUnitTests()
        {
            task = new DotnetPackTask();
            task.DotnetExecutable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationFromFluentInterfaceConfigurationTest()
        {
            task.Configuration("Release");
            task.ExecuteVoid(Context.Object);
            Assert.Equal(2, task.Arguments.Count);
            Assert.Equal("-c", task.Arguments[0]);
            Assert.Equal("Release", task.Arguments[1]);
        }

        [Fact]
        public void ConfigurationFromFluentInterfaceWithArgumentsTest()
        {
            task.WithArguments("--configuration", "Release");
            task.ExecuteVoid(Context.Object);
            Assert.Equal("--configuration", task.Arguments[0]);
            Assert.Equal("Release", task.Arguments[1]);
        }

        [Fact]
        public void ConfigurationFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.BuildConfiguration, null)).Returns("Release");
            task.ExecuteVoid(Context.Object);
            Assert.Equal("-c", task.Arguments[0]);
            Assert.Equal("Release", task.Arguments[1]);
        }
    }
}
