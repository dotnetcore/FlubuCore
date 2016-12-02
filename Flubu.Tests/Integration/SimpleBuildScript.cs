using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;

namespace Flubu.Tests.Integration
{
    public class SimpleBuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            ITargetFluentInterface test = session.CreateTarget("test");

            ITargetFluentInterface test1 = session.CreateTarget("test1")
                .DependsOn(test);
        }
    }
}
