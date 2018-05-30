using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace Flubu.Tests.TestData.BuildScripts
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
