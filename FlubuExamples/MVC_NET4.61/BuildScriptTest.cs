using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Packaging;
using FlubuCore.Scripting;

/// <summary>
/// Build script just for tests. If u want to try example run BuildScript.cs or BuildScriptWithDt.cs.
/// </summary>
public class BuildScriptTest : DefaultBuildScript
{
    public const string BaseExamplesPath = @".\FlubuExamples\";

    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.NUnitConsolePath, BaseExamplesPath + @"MVC_NET4.61\packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe");
        context.Properties.Set(BuildProps.ProductId, "FlubuExample");
        context.Properties.Set(BuildProps.ProductName, "FlubuExample");
        context.Properties.Set(BuildProps.SolutionFileName, BaseExamplesPath + "MVC_NET4.61\\FlubuExample.sln");
        context.Properties.Set(BuildProps.BuildConfiguration, "Release");
        context.Properties.Set(BuildProps.ProductRootDir, BaseExamplesPath + "MVC_NET4.61");
    }

    protected override void ConfigureTargets(ITaskContext session)
    {
        var loadSolution = session.CreateTarget("load.solution")
            .SetAsHidden()
            .AddTask(x => x.LoadSolutionTask());

        var projectVersion = session.CreateTarget("update.version")
            .SetAsHidden()
            .DependsOn(loadSolution)
            .Do(TargetFetchBuildVersion);

        session.CreateTarget("generate.commonassinfo")
           .DependsOn(projectVersion)
           .TaskExtensions().GenerateCommonAssemblyInfo();

        var compile = session.CreateTarget("compile")
            .AddTask(x => x.CompileSolutionTask())
            .DependsOn("generate.commonassinfo");
       
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

        context.Tasks().PackageTask(BaseExamplesPath + "MVC_NET4.61\\builds")
            .AddDirectoryToPackage(BaseExamplesPath + "MVC_NET4.61\\FlubuExample", "FlubuExample", false, new RegexFileFilter(@"^.*\.(svc|asax|aspx|config|js|html|ico|bat|cgn)$").NegateFilter())
            .AddDirectoryToPackage(BaseExamplesPath + "MVC_NET4.61\\FlubuExample\\Bin", "FlubuExample\\Bin", false, installBinFilters)
            .AddDirectoryToPackage(BaseExamplesPath + "MVC_NET4.61\\FlubuExample\\Content", "FlubuExample\\Content", true)
            .AddDirectoryToPackage(BaseExamplesPath + "MVC_NET4.61\\FlubuExample\\Images", "FlubuExample\\Images", true)
            .AddDirectoryToPackage(BaseExamplesPath + "MVC_NET4.61\\FlubuExample\\Scripts", "FlubuExample\\Scripts", true)
            .AddDirectoryToPackage(BaseExamplesPath + "MVC_NET4.61\\FlubuExample\\Views", "FlubuExample\\Views", true)
            .ZipPackage("FlubuExample.zip")
            .Execute(context);
    }
}