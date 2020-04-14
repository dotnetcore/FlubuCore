using FlubuCore.Context;
using FlubuCore.Context.Attributes;
using FlubuCore.Scripting;

namespace FlubuCore.ConsoleTestApp
{
    public class TestScript : DefaultBuildScript
    {
        [BuildProperty(DotNetBuildProps.OutputDir)] 
        public string Output{ get; set; } = "Output222";

        protected override void ConfigureTargets(ITaskContext context)
        {
            context.CreateTarget("Test").SetAsDefault().AddCoreTask(x => x.Build());
        }
    }
}
