using System;
using System.Collections.Generic;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Targeting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FlubuCore.Tests.Integration
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
            var session = _sp.GetRequiredService<IFlubuSession>();

            var bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasAllTargets(new List<string>() { "test" }, out _));
            Assert.True(session.TargetTree.HasAllTargets(new List<string>() { "test1" }, out _));

            var t = session.TargetTree.GetTarget("test");
            var t1 = session.TargetTree.GetTarget("test1");

            Assert.Equal(t.TargetName, t1.Dependencies.FirstOrDefault().TargetName);
        }

        [Fact]
        public void ConfigureTargetWithLinuxTasks()
        {
            var session = _sp.GetRequiredService<IFlubuSession>();

            var bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasAllTargets(new List<string>() { "Linux" }, out _));

            var t = (Target)session.TargetTree.GetTarget("Linux");

            Assert.Equal(2, t.TasksGroups.Count);
            Assert.Empty(t.Dependencies);
        }

        [Fact]
        public void ConfigureTargetWithIisTasks()
        {
            var session = _sp.GetRequiredService<IFlubuSession>();

            var bs = new SimpleBuildScript();
            bs.Run(session);

            Assert.True(session.TargetTree.HasAllTargets(new List<string>() { "IIS" }, out _));

            var t = (Target)session.TargetTree.GetTarget("IIS");

            Assert.Equal(2, t.TasksGroups.Count);
            Assert.Empty(t.Dependencies);
        }
    }
}