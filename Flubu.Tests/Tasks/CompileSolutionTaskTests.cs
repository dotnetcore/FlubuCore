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
        private CompileSolutionTask _task;

        private Mock<IFlubuEnviromentService> _flubuEnviroment;

        private Mock<IComponentProvider> _componentProvider;

        private Mock<IRunProgramTask> _runProgramTask;

        public CompileSolutionTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _componentProvider = new Mock<IComponentProvider>();
            _flubuEnviroment = new Mock<IFlubuEnviromentService>();
            _componentProvider.Setup(x => x.CreateFlubuEnviromentService()).Returns(_flubuEnviroment.Object);
            _runProgramTask = new Mock<IRunProgramTask>(MockBehavior.Loose);
        }

        [Fact]
        public void IfToolsVersionIsNotSpecifiedUseHighestOne()
        {
            SetupMSBuildVersions();
            SetupRunProgramTask();
            const string MSBuildPath = @"somewhere12.0\MSBuild.exe";
            _componentProvider.Setup(x => x.CreateRunProgramTask(MSBuildPath)).Returns(_runProgramTask.Object);

            _task = new CompileSolutionTask("x.sln", "Release", _componentProvider.Object);
            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(Context);

            _componentProvider.Verify();
        }

        [Fact]
        public void ExactToolsVersionWasFound()
        {
            SetupMSBuildVersions();
            SetupRunProgramTask();
            const string MSBuildPath = @"somewhere4.0\MSBuild.exe";
            _componentProvider.Setup(x => x.CreateRunProgramTask(MSBuildPath)).Returns(_runProgramTask.Object);

            _task = new CompileSolutionTask("x.sln", "Release", _componentProvider.Object);
            _task.ToolsVersion = new Version("4.0");
            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(Context);
            _componentProvider.Verify();
        }

        [Fact]
        public void ToolsVersionWasNotFoundButThereIsNewerOne()
        {
            SetupMSBuildVersions(include40: false);
            SetupRunProgramTask();
            const string MSBuildPath = "somewhere12.0\\MSBuild.exe";
            _componentProvider.Setup(x => x.CreateRunProgramTask(MSBuildPath)).Returns(_runProgramTask.Object);

            _task = new CompileSolutionTask("x.sln", "Release", _componentProvider.Object);
            _task.ToolsVersion = new Version("4.0");

            _task.UseSolutionDirAsWorkingDir = false;
            _task.Execute(Context);

            _componentProvider.Verify();
        }

        [Fact]
        public void ToolsVersionWasNotFoundAndThereIsNoNewerOne()
        {
            SetupMSBuildVersions(include40: false, include120: false);

            _task = new CompileSolutionTask("x.sln", "Release", _componentProvider.Object);
            _task.ToolsVersion = new Version("4.0");
            _task.UseSolutionDirAsWorkingDir = false;
            TaskExecutionException ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context));
            Assert.Equal("Requested MSBuild tools version 4.0 not found and there are no higher versions", ex.Message);

            _componentProvider.Verify();
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
