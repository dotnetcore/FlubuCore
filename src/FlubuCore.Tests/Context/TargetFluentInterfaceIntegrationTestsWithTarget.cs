using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Context
{
    public class TargetFluentInterfaceIntegrationTestsWithTarget
    {
        private readonly TargetFluentInterface _fluent;
        private readonly TaskContextInternal _context;
        private readonly Mock<ITaskFactory> _taskFactory;
        private readonly Target _target;
        private readonly TargetTree _targetTree;

        public TargetFluentInterfaceIntegrationTestsWithTarget()
        {
            _taskFactory = new Mock<ITaskFactory>();
            _targetTree = new TargetTree(null, null);
            _context = new TaskContextInternal(null, null, null, _targetTree, null, _taskFactory.Object,
                null);
            _target = new Target(new TargetTree(null, null), "TestTarget", null);

            _fluent = new TargetFluentInterface();
            _targetTree.AddTarget(_target);
            _fluent.Target = _target;
            _fluent.Context = _context;
            var coreTaskFluentInterface =
                new CoreTaskFluentInterface(new LinuxTaskFluentInterface(), new ToolsFluentInterface());
            coreTaskFluentInterface.Context = _context;
            _fluent.CoreTaskFluent = coreTaskFluentInterface;
            var taskFluentInterface = new TaskFluentInterface(new IisTaskFluentInterface(), new WebApiFluentInterface(),
                new GitFluentInterface(), new DockerFluentInterface(), new HttpClientFactory());
            taskFluentInterface.Context = _context;
            _fluent.TaskFluent = taskFluentInterface;
        }

        [Fact]
        public void When_AddDependencyConditionNotMeet()
        {
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            Mock<ITargetInternal> target2 = new Mock<ITargetInternal>();
            target2.Setup(x => x.TargetName).Returns("dep2");
            ITarget t = _fluent.DependsOn(target1.Object, target2.Object).When(c => false);
            Assert.NotNull(t);
            Assert.Equal(2, _target.Dependencies.Count);
            Assert.True(_target.Dependencies["dep2"].Skipped == true);
        }

        [Fact]
        public void When_AddDependencConditionMeet()
        {
            Mock<ITargetInternal> target2 = new Mock<ITargetInternal>();
            target2.Setup(x => x.TargetName).Returns("dep2");
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.DependsOn(target1.Object, target2.Object).When(c => true);
            Assert.NotNull(t);
            Assert.Equal(2, _target.Dependencies.Count);
        }

        [Fact]
        public void When_Add2DependenyConditionNotMeet()
        {
            Mock<ITargetInternal> target2 = new Mock<ITargetInternal>();
            target2.Setup(x => x.TargetName).Returns("dep2");
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.DependsOn(target1.Object).DependsOn(target2.Object).When(c => false);
            Assert.NotNull(t);
            Assert.True(_target.Dependencies.ContainsKey("dep"));
            Assert.True(_target.Dependencies.ContainsKey("dep2"));
            Assert.True(_target.Dependencies["dep2"].Skipped == true);
        }

        [Fact]
        public void When_AddDependencyConditionNull()
        {
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.DependsOn(target1.Object).When(null);
            Assert.NotNull(t);
            Assert.Single(_target.Dependencies);
        }

        [Fact]
        public void When_AddTaskConditionNotMet()
        {
            _taskFactory.Setup(x => x.Create<CleanOutputTask>()).Returns(new CleanOutputTask());
            _taskFactory.Setup(x => x.Create<CompileSolutionTask>()).Returns(new CompileSolutionTask(null));
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddTask(x => x.CompileSolutionTask()).AddTask(x => x.CleanOutputTask())
                .When(c => false);
            Assert.NotNull(t);
            Assert.Single(_target.TasksGroups);
            Assert.Single(_target.TasksGroups[0].Tasks);
        }

        [Fact]
        public void When_AddTaskConditionMet()
        {
            _taskFactory.Setup(x => x.Create<CleanOutputTask>()).Returns(new CleanOutputTask());
            _taskFactory.Setup(x => x.Create<CompileSolutionTask>()).Returns(new CompileSolutionTask(null));
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddTask(x => x.CompileSolutionTask()).AddTask(x => x.CleanOutputTask())
                .When(c => true);
            Assert.NotNull(t);
            Assert.Equal(2, _target.TasksGroups.Count);
            Assert.Single(_target.TasksGroups[0].Tasks);
            Assert.Single(_target.TasksGroups[1].Tasks);
        }

        [Fact]
        public void When_AddTaskAsyncConditionNotMet()
        {
            _taskFactory.Setup(x => x.Create<CleanOutputTask>()).Returns(new CleanOutputTask());
            _taskFactory.Setup(x => x.Create<CompileSolutionTask>()).Returns(new CompileSolutionTask(null));
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddTaskAsync(x => x.CompileSolutionTask()).AddTaskAsync(x => x.CleanOutputTask())
                .When(c => false);
            Assert.NotNull(t);
            Assert.Single(_target.TasksGroups);
            Assert.Single(_target.TasksGroups[0].Tasks);
        }

        [Fact]
        public void When_AddTaskAsyncConditionMet()
        {
            _taskFactory.Setup(x => x.Create<CleanOutputTask>()).Returns(new CleanOutputTask());
            _taskFactory.Setup(x => x.Create<CompileSolutionTask>()).Returns(new CompileSolutionTask(null));
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddTaskAsync(x => x.CompileSolutionTask()).AddTaskAsync(x => x.CleanOutputTask())
                .When(c => true);
            Assert.NotNull(t);
            Assert.Equal(2, _target.TasksGroups.Count);
            Assert.Single(_target.TasksGroups[0].Tasks);
            Assert.Single(_target.TasksGroups[1].Tasks);
        }

        [Fact]
        public void When_AddCoreTaskConditionNotMet()
        {
            _taskFactory.Setup(x => x.Create<DotnetBuildTask>()).Returns(new DotnetBuildTask());
            _taskFactory.Setup(x => x.Create<DotnetCleanTask>()).Returns(new DotnetCleanTask());
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddCoreTask(x => x.Build()).AddCoreTask(x => x.Clean())
                .When(c => false);
            Assert.NotNull(t);
            Assert.Single(_target.TasksGroups);
            Assert.Single(_target.TasksGroups[0].Tasks);
        }

        [Fact]
        public void When_AddCoreTaskConditionMet()
        {
            _taskFactory.Setup(x => x.Create<DotnetBuildTask>()).Returns(new DotnetBuildTask());
            _taskFactory.Setup(x => x.Create<DotnetCleanTask>()).Returns(new DotnetCleanTask());
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddCoreTask(x => x.Build()).AddCoreTask(x => x.Clean()).When(c => true);
            Assert.NotNull(t);
            Assert.Equal(2, _target.TasksGroups.Count);
            Assert.Equal(1, _target.TasksGroups[0].Tasks.Count);
            Assert.Equal(1, _target.TasksGroups[1].Tasks.Count);
        }

        [Fact]
        public void When_AddCoreTaskAsyncConditionNotMet()
        {
            _taskFactory.Setup(x => x.Create<DotnetBuildTask>()).Returns(new DotnetBuildTask());
            _taskFactory.Setup(x => x.Create<DotnetCleanTask>()).Returns(new DotnetCleanTask());
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddCoreTaskAsync(x => x.Build()).AddCoreTaskAsync(x => x.Clean())
                .When(c => false);
            Assert.NotNull(t);
            Assert.Single(_target.TasksGroups);
            Assert.Single(_target.TasksGroups[0].Tasks);
        }

        [Fact]
        public void When_AddCoreTaskAsyncConditionMet()
        {
            _taskFactory.Setup(x => x.Create<DotnetBuildTask>()).Returns(new DotnetBuildTask());
            _taskFactory.Setup(x => x.Create<DotnetCleanTask>()).Returns(new DotnetCleanTask());
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");
            ITarget t = _fluent.AddCoreTaskAsync(x => x.Build()).AddCoreTaskAsync(x => x.Clean()).When(c => true);
            Assert.NotNull(t);
            Assert.Equal(2, _target.TasksGroups.Count);
            Assert.Single(_target.TasksGroups[0].Tasks);
            Assert.Single(_target.TasksGroups[1].Tasks);
        }

        [Fact]
        public void ForEach_AddTask()
        {
            _taskFactory.Setup(x => x.Create<DotnetBuildTask>()).Returns(new DotnetBuildTask());
            _taskFactory.Setup(x => x.Create<DotnetCleanTask>()).Returns(new DotnetCleanTask());
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            target1.Setup(x => x.TargetName).Returns("dep");

            List<string> items = new List<string>() { "1", "2", "3", "4" };
            ITarget t = _fluent.ForEach(items, (s, target) =>
            {
                target.AddCoreTask(x => x.Build());
                target.Do(x => { });
            });

            Assert.Equal(8, _target.TasksGroups.Count);
            Assert.Single(_target.TasksGroups[0].Tasks);
            Assert.Single(_target.TasksGroups[1].Tasks);
        }

        [Fact]
        public void DepencdeOff_AddWithstring()
        {
            var t2 = new Target(_targetTree, "Target2", null);

            var fluent2 = new TargetFluentInterface();
            fluent2.Target = t2;
            fluent2.Context = _context;

            fluent2.DependenceOf("TestTarget");

            Assert.True(_target.Dependencies.ContainsKey("Target2"));
        }

        [Fact]
        public void DepencdeOff_AddWithTarget()
        {
            var t2 = new Target(_targetTree, "Target2", null);

            var fluent2 = new TargetFluentInterface();
            fluent2.Target = t2;
            fluent2.Context = _context;

            fluent2.DependenceOf(_fluent);

            Assert.True(_target.Dependencies.ContainsKey("Target2"));
        }

        [Fact]
        public void DepencdeOffAsync_AddWithstring()
        {
            var t2 = new Target(_targetTree, "Target2", null);

            var fluent2 = new TargetFluentInterface();
            fluent2.Target = t2;
            fluent2.Context = _context;

            fluent2.DependenceOfAsync("TestTarget");

            Assert.True(_target.Dependencies.ContainsKey("Target2"));
        }

        [Fact]
        public void DepencdeOffAsync_AddWithTarget()
        {
            var t2 = new Target(_targetTree, "Target2", null);

            var fluent2 = new TargetFluentInterface();
            fluent2.Target = t2;
            fluent2.Context = _context;

            fluent2.DependenceOfAsync(_fluent);

            Assert.True(_target.Dependencies.ContainsKey("Target2"));
        }
    }
}