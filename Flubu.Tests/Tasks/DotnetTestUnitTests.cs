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
  
    public class DotnetTestUnitTests : TaskUnitTestBase
    {
        private DotnetTestTask task;
        
        public DotnetTestUnitTests()
        {
            task = new DotnetTestTask();
            task.DotnetExecutable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceConfigurationTest()
        {
            task.Project("project");
            task.Configuration("Release");
            task.ExecuteVoid(Context.Object);
            Assert.Equal(3, task.Arguments.Count);
            Assert.Equal("project", task.Arguments[0]);
            Assert.Equal("-c", task.Arguments[1]);
            Assert.Equal("Release", task.Arguments[2]);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceWithArgumentsTest()
        {
            task.WithArguments("project");
            task.WithArguments("--configuration", "Release");
            task.ExecuteVoid(Context.Object);
            Assert.Equal("project", task.Arguments[0]);
            Assert.Equal("--configuration", task.Arguments[1]);
            Assert.Equal("Release", task.Arguments[2]);
        }

        [Fact]
        public void ConfigurationAndProjectFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.SolutionFileName, null)).Returns("project2");
            Properties.Setup(x => x.Get<string>(BuildProps.BuildConfiguration, null)).Returns("Release");
            task.ExecuteVoid(Context.Object);
            Assert.Equal("project2", task.Arguments[0]);
            Assert.Equal("-c", task.Arguments[1]);
            Assert.Equal("Release", task.Arguments[2]);
        }

        [Fact]
        public void ProjectIsFirstArgumentTest()
        {
            task.WithArguments("-c", "release", "somearg", "-sf");
            task.Project("project");
            task.ExecuteVoid(Context.Object);
            Assert.Equal("project", task.Arguments[0]);
        }
    }
}
