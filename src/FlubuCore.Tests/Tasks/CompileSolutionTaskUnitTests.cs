using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class CompileSolutionTaskUnitTests : TaskUnitTestBase
    {
        private readonly Mock<IFlubuEnvironmentService> _flubuEnviroment;
        private CompileSolutionTask _task;

        public CompileSolutionTaskUnitTests()
        {
            _flubuEnviroment = new Mock<IFlubuEnvironmentService>();
        }

        [Fact]
        public void IfToolsVersionIsNotSpecifiedUseHighestOne()
        {
            SetupMsBuildVersions();
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.ExecuteVoid(Context.Object);
        }

        [Fact]
        public void ExactToolsVersionWasFound()
        {
            SetupMsBuildVersions();
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.SetToolsVersion(new Version("4.0"));
            _task.ExecuteVoid(Context.Object);
        }

        [Fact]
        public void ToolsVersionWasNotFoundButThereIsNewerOne()
        {
            SetupMsBuildVersions(include40: false);
            SetupRunProgramTask();

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.SetToolsVersion(new Version("4.0"));
            _task.ExecuteVoid(Context.Object);
        }

        [Fact]
        public void ToolsVersionWasNotFoundAndThereIsNoNewerOne()
        {
            SetupMsBuildVersions(include40: false, include120: false);

            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.SetToolsVersion(new Version("4.0"));
            TaskExecutionException ex = Assert.Throws<TaskExecutionException>(() => _task.ExecuteVoid(Context.Object));
            Assert.Equal("Requested MSBuild tools version 4.0 not found and there are no higher versions", ex.Message);
        }

        [Fact]
        public void CheckArgs()
        {
            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.WithArguments("aaa");

            Assert.Single(_task.GetArguments());
        }

        [Fact]
        public void CheckArgsWhenExecuting()
        {
            SetupMsBuildVersions(include40: false, include120: false);
            SetupRunProgramTask();

            Tasks.Setup(x => x.RunProgramTask(It.IsAny<string>())).Returns(RunProgramTask.Object);
            _task = new CompileSolutionTask("x.sln", "Release", _flubuEnviroment.Object);
            _task.WithArguments("aaa");
            _task.Execute(Context.Object);
            Assert.Equal(4, _task.GetArguments().Count);
        }

        private void SetupMsBuildVersions(bool include40 = true, bool include120 = true)
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
            Tasks.Setup(x => x.RunProgramTask(It.IsAny<string>())).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WorkingFolder(".")).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WithArguments(It.IsAny<string>(), false)).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WithArguments(It.IsAny<string[]>())).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WithArguments("/p:Configuration=Release", false))
                .Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WithArguments("/p:Platform=Any CPU", false)).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WithArguments("/clp:NoSummary", false)).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(x => x.WithArguments("/maxcpucount:3", false)).Returns(RunProgramTask.Object);
        }
    }
}
