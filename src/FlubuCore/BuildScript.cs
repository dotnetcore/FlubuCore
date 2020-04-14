using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore
{
    /// <summary>
    /// Build script template.
    /// </summary>
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
            context.Properties.Set(BuildProps.ProductId, "Todo");
            context.Properties.Set(DotNetBuildProps.ProductName, "Todo");
            //// Adds some default targets like compile etc. Set To None if u don't want default targets.
            context.Properties.Set(DotNetBuildProps.DefaultTargets, DefaultTargets.Dotnet);
            context.Properties.Set(BuildProps.BuildConfiguration, "Release");
            context.Properties.Set(BuildProps.SolutionFileName, "..\\Todo.sln");
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            // Add custom targets to build and dependencies to default target if needed.
            session.CreateTarget("rebuild")
                .SetDescription("Rebuilds the project.")
                .SetAsDefault()
                .DependsOn("compile");

            //////// fetch.build.version and compile are default Dotnet targets.
            ////session.GetTarget("fetch.build.version");
        }
    }
}