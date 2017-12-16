using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Flubu.Tests
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

            targetTree.AddTarget("test target");

            Assert.True(targetTree.HasAllTargets(new List<string>() { "test target" }, out _));
        }

        [Fact]
        public void ExecuteTargetWithTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTask(new SimpleTask(new FileWrapper()));

            target1.ExecuteVoid(Context);
        }

        [Fact]
        public async Task ExecuteTargetWithAsyncTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTask(new FileWrapper()));

            await target1.ExecuteVoidAsync(Context);
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task ExecuteTargetWith2AsyncTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            var elapsed = sw.Elapsed;
            Assert.True(sw.ElapsedMilliseconds > 2000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 3999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public void ExecuteTargetWith2TaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTask(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            target1.ExecuteVoid(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 4000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task ExecuteTargetWith2Async1Sync2Test()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            target1.AddTask(new SimpleTaskWithDelay());

            Stopwatch sw = new Stopwatch();

            sw.Start();
            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 4000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 5999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task ExecuteTargetWith2Async1Sync2AsyncTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            target1.AddTask(new SimpleTaskWithDelay());
            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            await target1.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 6000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 7999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        public async Task DependsOnAsyncTargetTest()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target 3", "target 1", "target 2" } });

            var target1 = targetTree.AddTarget("target 1").AddTask(new SimpleTaskWithDelay());

            var target2 = targetTree.AddTarget("target 2").AddTask(new SimpleTaskWithDelay());

            var target3 = targetTree.AddTarget("target 3");
            target3.DependsOnAsync(target1, target2);
            Stopwatch sw = new Stopwatch();

            sw.Start();

            await target3.ExecuteVoidAsync(Context);
            sw.Stop();

            Assert.Equal(2, targetTree.DependenciesExecutedCount);

            Assert.True(sw.ElapsedMilliseconds > 2000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 3999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        public void DependsOnTargetTest()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target 3", "target 1", "target 2" } });
            var target1 = targetTree.AddTarget("target 1").AddTask(new SimpleTaskWithDelay());

            var target2 = targetTree.AddTarget("target 2").AddTask(new SimpleTaskWithDelay());

            var target3 = targetTree.AddTarget("target 3");
            target3.DependsOn(target1, target2);
            var dependencies = target3.Dependencies.ToList();
            Assert.Equal(2, dependencies.Count);
            Assert.Equal("target 1", dependencies[0].Key);
            Assert.Equal("target 2", dependencies[1].Key);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            target3.ExecuteVoid(Context);
            sw.Stop();

            Assert.Equal(2, targetTree.DependenciesExecutedCount);

            Assert.True(sw.ElapsedMilliseconds > 2000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task DoAsyncTargetTest()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target 3", "target 1", "target 2" } });

            var target1 = targetTree.AddTarget("target 1").DoAsync(DoWithDelay).DoAsync(DoWithDelay);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            target1.ExecuteVoid(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 2000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 3999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public async Task DoAsyncTargetTest2()
        {
            TargetTree targetTree = new TargetTree(ServiceProvider, new CommandArguments { TargetsToExecute = new List<string> { "target 3", "target 1", "target 2" } });

            var target1 = targetTree.AddTarget("target 1").DoAsync(DoWithDelay).DoAsync((Action<ITaskContextInternal>)DoWithDelay2);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            target1.ExecuteVoid(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 2000, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
            Assert.True(sw.ElapsedMilliseconds < 3999, $"Task took to complete {sw.ElapsedMilliseconds} miliseconds");
        }

        private async Task DoWithDelay(ITaskContext context)
        {
           await Task.Delay(2000);
        }

        private void DoWithDelay2(ITaskContext context)
        {
            Task.Delay(2000).Wait();
        }
    }
}