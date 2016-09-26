using Flubu.Context;
using Flubu.Extensions;
using Flubu.Scripting;
using Flubu.Targeting;

namespace Flubu.BuildScript
{
    public class MyBuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(ITaskSession session)
        {
            //session.Properties.Set(BuildProps.MSBuildToolsVersion, "4.0");
            //session.Properties.Set(BuildProps.NUnitConsolePath, @"packages\NUnit.Runners.2.6.2\tools\nunit-console.exe");

            session.Properties.Set(BuildProps.ProductId, "Flubu");
            session.Properties.Set(BuildProps.ProductName, "Flubu");
            
            //session.Properties.Set(BuildProps.SolutionFileName, "Flubu.sln");
            //session.Properties.Set(BuildProps.VersionControlSystem, VersionControlSystem.Mercurial);
        }

        protected override void ConfigureTargets(ITaskSession session)
        {
            Target.Create("init")
                .AddToTargetTree(session.TargetTree);

            Target.Create("compile")
                .PackageProject("Flubu")
                .PackageProject("dotnet-flubu")
                .AddToTargetTree(session.TargetTree);
        }
    }
}
