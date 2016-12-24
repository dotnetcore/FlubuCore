using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Packaging;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks.Testing;

/// <summary>
/// In this build script default targets(compile etc are included. 
/// Type "dotnet flubu -s=BuildScriptWithDt.cs help" in cmd to see
/// </summary>
public class BuildScriptWithDt : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.NUnitConsolePath,
            @"packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe");
        context.Properties.Set(BuildProps.ProductId, "FlubuExample");
        context.Properties.Set(BuildProps.ProductName, "FlubuExample");
        context.Properties.Set(BuildProps.SolutionFileName, "FlubuExample.sln");
        context.Properties.Set(BuildProps.BuildConfiguration, "Release");
        context.Properties.SetDefaultTargets(DefaultTargets.Dotnet);
    }

    protected override void ConfigureTargets(ITaskContext session)
    {
        var updateVersion = session.CreateTarget("update.version")
            .Do(TargetFetchBuildVersion)
            .SetAsHidden();

        var unitTest = session.CreateTarget("unit.tests")
            .SetDescription("Runs unit tests")
            .AddTask(x => x.NUnitTaskForNunitV3("FlubuExample.Tests"));

        var package = session.CreateTarget("Package")
            .SetDescription("Packages mvc example for deployment")
            .Do(TargetPackage);

        session.CreateTarget("Rebuild")
            .SetDescription("Rebuilds the solution.")
            .SetAsDefault()
            .DependsOn("compile") //// compile is included as one of the default targets.
            .DependsOn(unitTest, package);
    }

    public static void TargetFetchBuildVersion(ITaskContext context)
    {
        var version = context.Tasks().FetchBuildVersionFromFileTask().Execute(context);
     
        int svnRevisionNumber = 0; //in real scenario you would fetch revision number from subversion.
        int buildNumber = 0; // in real scenario you would fetch build version from build server.
        version = new Version(version.Major, version.Minor, buildNumber, svnRevisionNumber);
        context.Properties.Set(BuildProps.BuildVersion, version);
    }

    public static void TargetPackage(ITaskContext context)
    {
        FilterCollection installBinFilters = new FilterCollection();
        installBinFilters.Add(new RegexFileFilter(@".*\.xml$"));
        installBinFilters.Add(new RegexFileFilter(@".svn"));

        context.Tasks().PackageTask("builds")
            .AddDirectoryToPackage("FlubuExample", "FlubuExample", false, new RegexFileFilter(@"^.*\.(svc|asax|aspx|config|js|html|ico|bat|cgn)$").NegateFilter())
            .AddDirectoryToPackage("FlubuExample\\Bin", "FlubuExample\\Bin", false, installBinFilters)
            .AddDirectoryToPackage("FlubuExample\\Content", "FlubuExample\\Content", true)
            .AddDirectoryToPackage("FlubuExample\\Images", "FlubuExample\\Images", true)
            .AddDirectoryToPackage("FlubuExample\\Scripts", "FlubuExample\\Scripts", true)
            .AddDirectoryToPackage("FlubuExample\\Views", "FlubuExample\\Views", true)
            .ZipPackage("FlubuExample.zip")
            .Execute(context);
    }
}
