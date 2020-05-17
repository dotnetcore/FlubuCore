using FlubuCore.Context;
using FlubuCore.Context.Attributes;
using FlubuCore.Scripting;

namespace FlubuCore.ConsoleTestApp
{
    public class DefaultTestScript : DefaultBuildScript
    {        
        public string Output => RootDirectory.CombineWith("output222");
        
        protected override void ConfigureTargets(ITaskContext context)
        {
            context.CreateTarget("Test").SetAsDefault().AddTask(x => x.RunProgramTask("npm").WithArguments("list"));
        }
    }
}
