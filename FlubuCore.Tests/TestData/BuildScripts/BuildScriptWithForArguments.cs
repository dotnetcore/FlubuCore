using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;

namespace FlubuCore.Tests.TestData.BuildScripts
{
    public class BuildScriptWithForArguments : DefaultBuildScript
    {
        [FromArg("s")]
        public string SolutionFileName { get; set; }

        [FromArg("l")]
        public int Level { get; set; }

        [FromArg("sog")]
        public bool StayOrGo { get; set; }

        [FromArg("list")]
        public List<string> SomeList { get; set; }

        public string NoAttribute { get; set; }

        [Target("Test", 1, "Fsad")]
        protected void Test(ITarget target, int a, string b)
        {
        }

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }
    }
}
