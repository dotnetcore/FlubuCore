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
            var firstTarget = context.CreateTarget("FirstTarget").Do(t => { Console.WriteLine("display this to."); });

            var nested = context.CreateTarget("ShouldAlsoBeDisplayed").Do(t => { Console.WriteLine("Should also be displayed"); })
                .SetAsHidden()
                .DependsOn(firstTarget);

            var doExample = context.CreateTarget("DoExample")
                .Do(t => { throw new Exception("error on purpose."); });

            var doExample2 = context.CreateTarget("DoExample2")
                .DependsOn(nested)
                .Do(t => { Console.WriteLine("from doExample2"); });

            var doExample3 = context.CreateTarget("DoExample3")
                .Do(t => { Console.WriteLine("from doExample3"); });

            context.CreateTarget("Test")
                .SetAsDefault()
                .AddTask(t => t.Do(x => { Console.WriteLine("from Test"); }))
                .DependsOnAsync(doExample2, doExample)
                .DependsOn(doExample3);

            context.CreateTarget("Test2")
                .SetAsDefault()
                .AddTask(t => t.Do(x => { Console.WriteLine("from Test"); }));
        }
    }
}
