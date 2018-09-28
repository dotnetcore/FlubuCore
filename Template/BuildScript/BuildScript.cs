using FlubuCore.Context;
using FlubuCore.Scripting;
using System;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
            context.Properties.Set(BuildProps.SolutionFileName, "Todo");
        }

        protected override void ConfigureTargets(ITaskContext context)
        {
            var compile = context.CreateTarget("compile")
                .SetDescription("Compiles the solution.")
                .AddCoreTask(x => x.Build());
        }
    }
}
