using System;
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

            //// Assert.Equal(t.TargetName, t1.Dependencies.FirstOrDefault());
        }
    }
}
