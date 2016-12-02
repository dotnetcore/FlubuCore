using System;
using System.Collections.Generic;
using System.IO;
using Flubu;
using Flubu.Builds;
using Flubu.Builds.Tasks.NuGetTasks;
using Flubu.Builds.Tasks.TestingTasks;
using Flubu.Builds.VSSolutionBrowsing;
using Flubu.Packaging;
using Flubu.Targeting;
using Flubu.Tasks.Iis;
using Flubu.Tasks.Iis.Iis7;
using Flubu.Tasks.Processes;
//css_imp BuildScripts\\ExampleCustomTask.cs;
namespace BuildScripts
{
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(TaskSession session)
        {
            session.Properties.Set(BuildProps.MSBuildToolsVersion, "4.0");
            session.Properties.Set(BuildProps.NUnitConsolePath, @"packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe");
            session.Properties.Set(BuildProps.ProductId, "FlubuExample");
            session.Properties.Set(BuildProps.ProductName, "FlubuExample");
            session.Properties.Set(BuildProps.SolutionFileName, "FlubuExample.sln");
            session.Properties.Set(BuildProps.VersionControlSystem, VersionControlSystem.Mercurial);
        }

        protected override void ConfigureTargets(TargetTree targetTree, ICollection<string> args)
        {
            ////compile is a flubu built in target.
            targetTree.AddTarget("rebuild")
                .SetDescription("Rebuilds the project, runs tests and packages the build products.")
                .SetAsDefault()
                .DependsOn("compile", "unit.tests", "package");

            ////load solution is a flubu built in target.
            targetTree.AddTarget("unit.tests")
               .SetDescription("Runs unit tests on the project")
               .Do(x => TargetRunTests(x)).DependsOn("load.solution");
        }

        private static void TargetRunTests(ITaskContext context)
        {
            var task = NUnitTask.ForNunitV3("FlubuExample.Tests");
            task.Execute(context);
        }
    }
}
