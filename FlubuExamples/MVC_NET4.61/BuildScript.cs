using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Packaging;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks.Testing;


public class BuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.NUnitConsolePath,
            @"packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe");
        context.Properties.Set(BuildProps.ProductId, "FlubuExample");
        context.Properties.Set(BuildProps.ProductName, "FlubuExample");
        context.Properties.Set(BuildProps.SolutionFileName, "FlubuExample.sln");
        context.Properties.Set(BuildProps.BuildConfiguration, "Release");

    }

    protected override void ConfigureTargets(ITaskContext session)
    {
        var loadSolution = session.CreateTarget("load.solution")
            .SetAsHidden()
            .AddTask(x => x.LoadSolutionTask());

        var projectVersion = session.CreateTarget("update.version")
            .DependsOn(loadSolution)
            .Do(TargetFetchBuildVersion);

        session.CreateTarget("generate.commonassinfo")
           .DependsOn(projectVersion)
           .TaskExtensions().GenerateCommonAssemblyInfo();

        var compile = session.CreateTarget("compile")
            .AddTask(x => x.CompileSolutionTask())
            .DependsOn("generate.commonassinfo");

        //// Just an example of Do.  It would be a better way to use AddTask() method to run tests. 
        var unitTest = session.CreateTarget("unit.tests")
            .AddTask(x => x.NUnitTaskForNunitV3("FlubuExample.Tests"));

        var package = session.CreateTarget("Package")
            .Do(TargetPackage);

        session.CreateTarget("Rebuild")
            .SetAsDefault()
            .DependsOn(compile, unitTest, package);
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
            .AddDirectoryToPackage("FlubuExample", "FlubuExample", "FlubuExample", false, new RegexFileFilter(@"^.*\.(svc|asax|aspx|config|js|html|ico|bat|cgn)$").NegateFilter())
            .AddDirectoryToPackage("Bin", "FlubuExample\\Bin", "FlubuExample\\Bin", false, installBinFilters)
            .AddDirectoryToPackage("Content", "FlubuExample\\Content", "FlubuExample\\Content", true)
            .AddDirectoryToPackage("Images", "FlubuExample\\Images", "FlubuExample\\Images", true)
            .AddDirectoryToPackage("Scripts", "FlubuExample\\Scripts", "FlubuExample\\Scripts", true)
            .AddDirectoryToPackage("Views", "FlubuExample\\Views", "FlubuExample\\Views", true)
            .ZipPrefix("FlubuExample")
            .Execute(context);
    }
}
