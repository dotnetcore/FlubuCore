using System;
using System.Linq;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flubu.Tests
{
    public class TargetTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IServiceProvider _provider;

        public TargetTests()
        {
            _services
                .AddCoreComponents()
                .AddArguments(new string[] { });

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
    }
}