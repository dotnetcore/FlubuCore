using System;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Targeting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flubu.Tests.Integration
{
    [Collection(nameof(IntegrationTestCollection))]
    public class ConfigureTargetsTests
    {
        private readonly IServiceProvider _sp;

        public ConfigureTargetsTests(IntegrationTestFixture fixture)
        {
            _sp = fixture.ServiceProvider;
        }

        [Fact]
        public void ConfigureSimpleTarget()
        {
            ITaskSession session = _sp.GetRequiredService<ITaskSession>();

            SimpleBuildScript bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasTarget("test"));
            Assert.True(session.TargetTree.HasTarget("test1"));

            ITarget t = session.TargetTree.GetTarget("test");
            ITarget t1 = session.TargetTree.GetTarget("test1");

            Assert.Equal(t.TargetName, t1.Dependencies.FirstOrDefault());
        }

        [Fact]
        public void ConfigureTargetWithExtensionTasks()
        {
            ITaskSession session = _sp.GetRequiredService<ITaskSession>();

            SimpleBuildScript bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasTarget("extensions"));

            Target t = (Target)session.TargetTree.GetTarget("extensions");

            Assert.Equal(2, t.Tasks.Count);
        }

        [Fact]
        public void ConfigureTargetWithExtensionTasksAndDependsOn()
        {
            ITaskSession session = _sp.GetRequiredService<ITaskSession>();

            SimpleBuildScript bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasTarget("package"));

            Target t = (Target)session.TargetTree.GetTarget("package");

            Assert.Equal(4, t.Tasks.Count);

            Assert.Equal(2, t.Dependencies.Count);
            Assert.Equal("init", t.Dependencies.First());
            Assert.Equal("restore", t.Dependencies.Last());
        }

        [Fact]
        public void ConfigureTargetWithLinuxTasks()
        {
            ITaskSession session = _sp.GetRequiredService<ITaskSession>();

            SimpleBuildScript bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasTarget("Linux"));

            Target t = (Target)session.TargetTree.GetTarget("Linux");

            Assert.Equal(2, t.Tasks.Count);
            Assert.Equal(0, t.Dependencies.Count);
        }

        [Fact]
        public void ConfigureTargetWithIISTasks()
        {
            ITaskSession session = _sp.GetRequiredService<ITaskSession>();

            SimpleBuildScript bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasTarget("IIS"));

            Target t = (Target)session.TargetTree.GetTarget("IIS");

            Assert.Equal(2, t.Tasks.Count);
            Assert.Equal(0, t.Dependencies.Count);
        }
    }
}
