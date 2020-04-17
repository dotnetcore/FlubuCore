using System;
using FlubuCore.Context;
using FlubuCore.Context.Attributes;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Scripting;

namespace FlubuCore.ConsoleTestApp
{
    public class TestScriptWithAsync : DefaultBuildScript
    {               
        protected override void ConfigureTargets(ITaskContext context)
        {
            var doExample = context.CreateTarget("DoExample")
                .Do(t => { throw new Exception("error on purpose."); });

            var doExample2 = context.CreateTarget("DoExample2")
                .Do(t => { Console.WriteLine("from doExample2"); });

            var doExample3 = context.CreateTarget("DoExample3")
                .Do(t => { Console.WriteLine("from doExample3"); });

            context.CreateTarget("Test")
                .SetAsDefault()
                .AddTask(t => t.Do(x => { Console.WriteLine("from Test"); }))
                .DependsOnAsync(doExample, doExample2)
                .DependsOn(doExample3);
        }
    }
}
