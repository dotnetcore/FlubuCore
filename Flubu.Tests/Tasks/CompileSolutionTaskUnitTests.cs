using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Services;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class CompileSolutionTaskUnitTests
    {
        private readonly Mock<IFlubuEnviromentService> _flubuEnviroment;
        private readonly Mock<IRunProgramTask> _runProgramTask;
        private readonly Mock<ITaskContext> _context;
        private readonly Mock<ITaskFluentInterface> _taskFluentInterface;
        private CompileSolutionTask _task;

        public CompileSolutionTaskUnitTests()
        {
            _flubuEnviroment = new Mock<IFlubuEnviromentService>();
            _runProgramTask = new Mock<IRunProgramTask>(MockBehavior.Loose);
            _context = new Mock<ITaskContext>();
            _taskFluentInterface = new Mock<ITaskFluentInterface>();
            _context.Setup(x => x.Tasks()).Returns(_taskFluentInterface.Object);
        }

        [Fact]
        public void IfToolsVersionIsNotSpecifiedUseHighestOne()
        {
            SetupMSBuildVersions();
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(_context.Object);
        }

        [Fact]
        public void ExactToolsVersionWasFound()
        {
            SetupMSBuildVersions();
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ToolsVersion = new Version("4.0");
            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(_context.Object);
        }

        [Fact]
        public void ToolsVersionWasNotFoundButThereIsNewerOne()
        {
            SetupMSBuildVersions(include40: false);
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ToolsVersion = new Version("4.0");

            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(_context.Object);
        }

        [Fact]
        public void ToolsVersionWasNotFoundAndThereIsNoNewerOne()
        {
            SetupMSBuildVersions(include40: false, include120: false);

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ToolsVersion = new Version("4.0");
            _task.UseSolutionDirAsWorkingDir = false;
            TaskExecutionException ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(_context.Object));
            Assert.Equal("Requested MSBuild tools version 4.0 not found and there are no higher versions", ex.Message);
        }

        private void SetupMSBuildVersions(bool include40 = true, bool include120 = true)
        {
            IDictionary<Version, string> msbuilds = new SortedDictionary<Version, string>();
            msbuilds.Add(new Version("2.0"), "somewhere2.0");
            if (include40)
                msbuilds.Add(new Version("4.0"), "somewhere4.0");
            if (include120)
                msbuilds.Add(new Version("12.0"), "somewhere12.0");

            _flubuEnviroment.Setup(x => x.ListAvailableMSBuildToolsVersions()).Returns(msbuilds);
        }

        private void SetupRunProgramTask()
        {
            _taskFluentInterface.Setup(x => x.RunProgramTask(It.IsAny<string>())).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("x.sln")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/p:Configuration=Release")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/p:Platform=Any CPU")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/consoleloggerparameters:NoSummary")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/maxcpucount:3")).Returns(_runProgramTask.Object);
        }
    }
}
