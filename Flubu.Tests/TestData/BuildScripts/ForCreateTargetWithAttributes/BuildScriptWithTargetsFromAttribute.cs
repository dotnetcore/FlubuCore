using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;

namespace Flubu.Tests.TestData.BuildScripts.ForCreateTargetWithAttributes
{
    public class BuildScriptWithTargetsFromAttribute : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target("Target1", "value1", "value2")]
        [Target("Target2", "value2", "value3")]
        public void TestTargets(ITargetFluentInterface target, string param, string param2)
        {
            target.AddTask(x => x.CompileSolutionTask(param, param2));
        }

        [Target("Target3")]
        public void TestTargets2(ITargetFluentInterface target)
        {
            target.AddTask(x => x.CompileSolutionTask());
        }
    }
}
