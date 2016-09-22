using System.Linq;
using Flubu.Targeting;
using Xunit;

namespace Flubu.Tests
{
    public class TargetTests
    {
        [Fact]
        public void AddToTargetTreeTest()
        {
            var targetTree = new TargetTree();

            new Target("test target")
                .AddToTargetTree(targetTree);

            Assert.True(targetTree.HasTarget("test target"));
        }

        [Fact]
        public void DependsOnTargetTest()
        {
            var target1 = new Target("target 1");

            var target2 = new Target("target 2");

            var target3 = new Target("target 3");
            target3.DependsOn(target1, target2);
            var dependencies = target3.Dependencies.ToList();
            Assert.Equal(2, dependencies.Count);
            Assert.Equal("target 1", dependencies[0]);
            Assert.Equal("target 2", dependencies[1]);
        }
    }
}