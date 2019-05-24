using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore.Tests.TestData.BuildScripts.ForCreateTargetWithAttributes
{
    public class BuildScriptNoTargetsWithAttribute : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            session.CreateTarget("Test").AddTask(x => x.CompileSolutionTask());
        }
    }
}
