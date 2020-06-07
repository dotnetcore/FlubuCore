using FlubuCore.Context;
using FlubuCore.Context.Attributes;
using FlubuCore.Context.Attributes.BuildProperties;
using FlubuCore.Scripting;

namespace FlubuCore.ConsoleTestApp
{
    public class DefaultTestScript : DefaultBuildScript
    {
        [SolutionFileName]
        public string SolutionFileName => "flubu.sln";

        public string Output => RootDirectory.CombineWith("output222");
        
        protected override void ConfigureTargets(ITaskContext context)
        {
            context.CreateTarget("Test")
                .SetAsDefault()
                .AddCoreTask(x => x.Restore().WorkingFolder("../../../.."));
        }
    }
}
