using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class CompileSolutionTaskTests : FlubuTestBase
    {
        private readonly Mock<IFlubuEnviromentService> _flubuEnviroment;
        private readonly Mock<RunProgramTask> _runProgramTask;
        private readonly Mock<ITaskContext> _context;
        private CompileSolutionTask _task;

        public CompileSolutionTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _flubuEnviroment = new Mock<IFlubuEnviromentService>();
            _runProgramTask = new Mock<RunProgramTask>(MockBehavior.Loose);
            _context = new Mock<ITaskContext>();
        }

        [Fact(Skip = "Fix test. Mock class")]
        public void IfToolsVersionIsNotSpecifiedUseHighestOne()
        {
            SetupMSBuildVersions();
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.UseSolutionDirAsWorkingDir = false;
            _context.Setup(i => i.Tasks()).Returns(new TaskFluentInterface());
            _context.Setup(i => i.CreateTask<RunProgramTask>()).Returns(_runProgramTask.Object);
            _task.Execute(_context.Object);
        }

        [Fact(Skip = "Fix test. Mock class")]
        public void ExactToolsVersionWasFound()
        {
            SetupMSBuildVersions();
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ToolsVersion = new Version("4.0");
            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(Context);
        }

        [Fact(Skip = "Fix test. Mock class")]
        public void ToolsVersionWasNotFoundButThereIsNewerOne()
        {
            SetupMSBuildVersions(include40: false);
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ToolsVersion = new Version("4.0");

            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(Context);
        }

        [Fact]
        public void ToolsVersionWasNotFoundAndThereIsNoNewerOne()
        {
            SetupMSBuildVersions(include40: false, include120: false);

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ToolsVersion = new Version("4.0");
            _task.UseSolutionDirAsWorkingDir = false;
            TaskExecutionException ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context));
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
            _runProgramTask.Setup(x => x.WithArguments("x.sln")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/p:Configuration=Release")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/p:Platform=Any CPU")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/consoleloggerparameters:NoSummary")).Returns(_runProgramTask.Object);
            _runProgramTask.Setup(x => x.WithArguments("/maxcpucount:3")).Returns(_runProgramTask.Object);
        }
    }
}
