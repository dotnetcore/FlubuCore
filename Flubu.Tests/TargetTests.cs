using System;
using System.Diagnostics;
using System.Linq;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flubu.Tests
{
    [Collection(nameof(FlubuTestCollection))]
    public class TargetTests : FlubuTestBase
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IServiceProvider _provider;

        public TargetTests(FlubuTestFixture fixture) : base(fixture.LoggerFactory)
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

            Assert.True(targetTree.HasTarget("test target"));
        }

        [Fact]
        public void DependsOnTargetTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            var target2 = targetTree.AddTarget("target 2");

            var target3 = targetTree.AddTarget("target 3");
            target3.DependsOn(target1, target2);
            var dependencies = target3.Dependencies.ToList();
            Assert.Equal(2, dependencies.Count);
            Assert.Equal("target 1", dependencies[0]);
            Assert.Equal("target 2", dependencies[1]);
        }

        [Fact]
        public void ExecuteTargetWithTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTask(new SimpleTask());

            target1.ExecuteVoid(Context);
        }

        [Fact]
        public void ExecuteTargetWithAsyncTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTask());

            target1.ExecuteVoid(Context);
        }


        [Fact]
        public void ExecuteTargetWith2AsyncTaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            target1.ExecuteVoid(Context);
            sw.Stop();

            var elapsed = sw.Elapsed;
            Assert.True(sw.ElapsedMilliseconds > 1000);
            Assert.True(sw.ElapsedMilliseconds < 1999);
        }

        [Fact]
        public void ExecuteTargetWith2TaskTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTask(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            target1.ExecuteVoid(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 2000);
        }

        [Fact]
        public void ExecuteTargetWith2Async1Sync2Test()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            target1.AddTask(new SimpleTaskWithDelay());
           
            Stopwatch sw = new Stopwatch();

            sw.Start();
            target1.ExecuteVoid(Context);
            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds > 2000);
            Assert.True(sw.ElapsedMilliseconds < 2999);
        }

        [Fact]
        public void ExecuteTargetWith2Async1Sync2AsyncTest()
        {
            TargetTree targetTree = _provider.GetService<TargetTree>();

            var target1 = targetTree.AddTarget("target 1");

            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            target1.AddTask(new SimpleTaskWithDelay());
            target1.AddTaskAsync(new SimpleTaskWithDelay(), new SimpleTaskWithDelay());
            Stopwatch sw = new Stopwatch();

            sw.Start();
            target1.ExecuteVoid(Context);
            sw.Stop();
            
            Assert.True(sw.ElapsedMilliseconds > 3000);
            Assert.True(sw.ElapsedMilliseconds < 3999);
        }
    }
}