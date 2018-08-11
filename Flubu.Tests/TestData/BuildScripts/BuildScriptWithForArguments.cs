using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace Flubu.Tests.TestData.BuildScripts
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

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }
    }
}
