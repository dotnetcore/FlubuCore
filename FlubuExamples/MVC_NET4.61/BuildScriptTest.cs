using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;

/// <summary>
/// Build script just for tests. If u want to try example run BuildScript.cs
/// </summary>
public class BuildScriptTest : DefaultBuildScript
{
    public const string BaseExamplesPath = @"..\FlubuExamples\";

    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.NUnitConsolePath,
            BaseExamplesPath + @"MVC_NET4.61\packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe");
        context.Properties.Set(BuildProps.ProductId, "FlubuExample");
        context.Properties.Set(BuildProps.ProductName, "FlubuExample");
        context.Properties.Set(BuildProps.SolutionFileName, BaseExamplesPath + "MVC_NET4.61\\FlubuExample.sln");
        context.Properties.Set(BuildProps.BuildConfiguration, "Release");
    }

    protected override void ConfigureTargets(ITaskContext session)
    {
        var loadSolution = session.CreateTarget("load.solution")
            .SetAsHidden()
            .AddTask(x => x.LoadSolutionTask());

        var compile = session.CreateTarget("compile")
            .AddTask(x => x.CompileSolutionTask())
            .DependsOn(loadSolution);

        //// Just an example of Do.  It would be a better way to use AddTask() method to run tests. 
        var unitTest = session.CreateTarget("Sample")
            .Do(RunTests);

        session.CreateTarget("Rebuild")
            .SetAsDefault()
            .DependsOn(compile, unitTest);
    }
    
    /// <param name="context"></param>
    public static void RunTests(ITaskContext context)
    {
        ////Just an example. You can execute any custom code.
        context.Tasks().NUnitTaskForNunitV3("FlubuExample.Tests").Execute(context);
    }
}