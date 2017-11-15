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
  
    public class DotnetRestoreUnitTests : TaskUnitTestBase
    {
        private DotnetRestoreTask task;
        
        public DotnetRestoreUnitTests()
        {
            task = new DotnetRestoreTask();
            task.DotnetExecutable("dotnet");
            Tasks.Setup(x => x.RunProgramTask("dotnet")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceConfigurationTest()
        {
            task.Project("project");
            task.ExecuteVoid(Context.Object);
            Assert.Equal(1, task.Arguments.Count);
            Assert.Equal("project", task.Arguments[0]);
        }

        [Fact]
        public void ConfigurationAndProjectFromFluentInterfaceWithArgumentsTest()
        {
            task.WithArguments("project");
            task.ExecuteVoid(Context.Object);
            Assert.Equal("project", task.Arguments[0]);
        }

        [Fact]
        public void ConfigurationAndProjectFromBuildPropertiesTest()
        {
            Properties.Setup(x => x.Get<string>(BuildProps.SolutionFileName, null)).Returns("project2");
            task.ExecuteVoid(Context.Object);
            Assert.Equal(1, task.Arguments.Count);
            Assert.Equal("project2", task.Arguments[0]);
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
