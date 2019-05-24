using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Tests.Integration
{
    [Collection(nameof(FlubuTestCollection))]
    public class TargetTests : FlubuTestBase
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IServiceProvider _provider;

        public TargetTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _services
                .AddCoreComponents()
                .AddCommandComponents()
                .AddArguments(new string[] { })
                .AddTasks();

            _provider = _services.BuildServiceProvider();
        }

        [Fact]
        public void AddToTargetTreeTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            targetTree.AddTarget("test.target");

            Assert.True(targetTree.HasAllTargets(new List<string>() { "test.target" }, out _));
        }

        [Fact]
        public void ExecuteTargetWithTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTask(null, new SimpleTask(new FileWrapper()));

            target1.ExecuteVoid(Context);
        }

        [Fact]
        public async Task ExecuteTargetWithAsyncTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTaskAsync(null, new SimpleTask(new FileWrapper()));

            await target1.ExecuteVoidAsync(Context);
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task ExecuteTargetWith2AsyncTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTaskAsync(null, new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            var elapsed = sw.Elapsed;
            Assert.True(sw.ElapsedMilliseconds > 2999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 5999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public void ExecuteTargetWith2TaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTask(null, new SimpleTaskWithDelay(500), new SimpleTaskWithDelay(500));
            Stopwatch sw = new Stopwatch();

            sw.Start();
            target1.ExecuteVoid(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task ExecuteTargetWith2Async1Sync2Test()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTaskAsync(null, new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            target1.AddTask(null, new SimpleTaskWithDelay());

            Stopwatch sw = new Stopwatch();

            sw.Start();
            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 5999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 8999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task ExecuteTargetWith2Async1Sync2AsyncTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTaskAsync(null, new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            target1.AddTask(null, new SimpleTaskWithDelay());
            target1.AddTaskAsync(null, new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 8999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 11999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact(Skip = "Test is unstable.")]
        [Trait("Category", "OnlyWindows")]
        public async Task DependsOnAsyncTargetTest()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target3", "target1", "target2" } });

            var target1 = targetTree.AddTarget("target1").AddTask(null, new SimpleTaskWithDelay());

            var target2 = targetTree.AddTarget("target2").AddTask(null, new SimpleTaskWithDelay());

            var target3 = targetTree.AddTarget("target3");
            target3.DependsOnAsync(target1, target2);
            Stopwatch sw = new Stopwatch();

            sw.Start();

            await target3.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.Equal(2, targetTree.DependenciesExecutedCount);

            Assert.True(sw.ElapsedMilliseconds > 3000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 5999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public void DependsOnTargetTest()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target3", "target1", "target2" } });
            var target1 = targetTree.AddTarget("target1").AddTask(null, new SimpleTaskWithDelay(500));

            var target2 = targetTree.AddTarget("target2").AddTask(null, new SimpleTaskWithDelay(500));

            var target3 = targetTree.AddTarget("target3");
            target3.DependsOn(target1, target2);
            var dependencies = target3.Dependencies.ToList();
            Assert.Equal(2, dependencies.Count);
            Assert.Equal("target1", dependencies[0].Key);
            Assert.Equal("target2", dependencies[1].Key);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            target3.ExecuteVoid(Context);
            sw.Stop();

            Assert.Equal(2, targetTree.DependenciesExecutedCount);

            Assert.True(sw.ElapsedMilliseconds > 999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task DoAsyncTargetTest()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target3", "target1", "target2" } });

            var target1 = targetTree.AddTarget("target1").DoAsync(DoWithDelay).DoAsync(DoWithDelay);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 3000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 5999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        public void ForMember_PropertyTestWithDoTask_SuccesfullArgumentPassThrough()
        {
            var doTask = new DoTask2<string>(ForMemberDoTest, "test");
            Context.ScriptArgs.Add("s", "value from arg");
            doTask.ForMember(x => x.Param, "-s");
            doTask.Execute(Context);
        }

        [Fact]
        public void Must_ConditionNotMeet_ThrowsException()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTask(null, new SimpleTask(new FileWrapper())).Must(() => false);

            var ex = Assert.Throws<TaskExecutionException>(() => target1.ExecuteVoid(Context));
            Assert.Equal(50, ex.ErrorCode);
        }

        [Fact]
        public void Must_ConditionMeet_ExecutesTarget()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target1");

            target1.AddTask(null, new SimpleTask(new FileWrapper())).Must(() => true);

            target1.ExecuteVoid(Context);
        }

        private void ForMemberDoTest(ITaskContext context, string param)
        {
            Assert.Equal("value from arg", param);
        }

        private async Task DoWithDelay(ITaskContext context)
        {
           await Task.Delay(3000);
        }
    }
}