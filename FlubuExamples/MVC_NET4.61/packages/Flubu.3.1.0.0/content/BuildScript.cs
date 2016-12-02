using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flubu;
using Flubu.Builds;
using Flubu.Targeting;

namespace BuildScripts
{
    /// <summary>
    /// Build script template.
    /// </summary>
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(TaskSession session)
        {
            session.Properties.Set(BuildProps.MSBuildToolsVersion, "4.0");
            session.Properties.Set(BuildProps.ProductId, "Todo");
            session.Properties.Set(BuildProps.ProductName, "Todo");
            session.Properties.Set(BuildProps.SolutionFileName, "..\\Todo.sln");
            session.Properties.Set(BuildProps.ProjectVersionFileName, "..\\Todo.ProjectVersion.txt");
            session.Properties.Set(BuildProps.VersionControlSystem, VersionControlSystem.Subversion);
        }

        protected override void ConfigureTargets(TargetTree targetTree, ICollection<string> args)
        {
            //Add custom targets to build and dependencies to default target if needed.
            targetTree.AddTarget("rebuild")
                .SetDescription("Rebuilds the project.")
                .SetAsDefault()
                .DependsOn("compile");

            targetTree.GetTarget("fetch.build.version")
                .Do(TargetFetchBuildVersion);
        }

        private static void TargetFetchBuildVersion(ITaskContext context)
        {
            Version version = BuildTargets.FetchBuildVersionFromFile(context);
            context.Properties.Set(BuildProps.BuildVersion, version);
            context.WriteInfo("The build version will be {0}", version);
        }
    }
}
