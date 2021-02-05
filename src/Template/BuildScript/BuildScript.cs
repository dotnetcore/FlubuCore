using FlubuCore.Context;
using FlubuCore.Scripting;
using System;
using FlubuCore.Context.Attributes.BuildProperties;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        [SolutionFileName]
        public string SolutionFileName { get; set; } = "Todo";

        protected override void ConfigureTargets(ITaskContext context)
        {
            var compile = context.CreateTarget("compile")
                .SetDescription("Compiles the solution.")
                .AddCoreTask(x => x.Build());
        }
    }
}
