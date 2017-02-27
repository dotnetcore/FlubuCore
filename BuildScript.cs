using System;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Nuget;
using System.IO;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.CompanyName, "Flubu");
        context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2016 Flubu");
        context.Properties.Set(BuildProps.ProductId, "FlubuCore");
        context.Properties.Set(BuildProps.ProductName, "FlubuCore");
        context.Properties.Set(BuildProps.BuildDir, "output");
    }

    protected override void ConfigureTargets(ITaskContext context)
    {
        var buildVersion = context.CreateTarget("buildVersion")
            .SetAsHidden()
            .SetDescription("Fetches flubu version from FlubuCore.ProjectVersion.txt file.")
            .AddTask(x => x.FetchBuildVersionFromFileTask());

        var compile = context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "Flubu.Tests/Flubu.Tests.csproj")
                        .AdditionalProp("dependencies.FlubuCore", "dependencies.dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("Flubu.sln"))
            .AddCoreTask(x => x.ExecuteDotnetTask("build").WithArguments("Flubu.sln"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack")
                        .WithArguments("FlubuCore", "-c", "Release")
                        .WithArguments("-o", "..\\output"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack")
                        .WithArguments("dotnet-flubu", "-c", "Release")
                        .WithArguments("-o", "..\\output"))
            .DependsOn(buildVersion);

       var flubuRunnerMerge = context.CreateTarget("merge")
            .SetDescription("Merge's all assemblyes into .net flubu console application")
            .Do(TargetMerge);

        var flubuTests = context.CreateTarget("test")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.ExecuteDotnetTask("test").WithArguments("Flubu.Tests\\Flubu.Tests.csproj"));

        var nugetPublish = context.CreateTarget("nuget.publish")
            .Do(PublishNuGetPackage).
            DependsOn(buildVersion);

        var packageFlubuRunner =context.CreateTarget("package.FlubuRunner")
            .SetDescription("Packages .net 4.6 Flubu runner into zip.")
            .Do(TargetPackageFlubuRunner);

        context.CreateTarget("rebuild")
            .SetDescription("Rebuilds the solution")
            .SetAsDefault()
            .DependsOn(compile, flubuTests);

        context.CreateTarget("rebuild.server")
            .SetDescription("Rebuilds the solution and publishes nuget packages.")
            .DependsOn(compile, flubuTests, flubuRunnerMerge, nugetPublish, packageFlubuRunner);
    }

    private static void TargetPackageFlubuRunner(ITaskContext context)
    {
         context.Tasks().PackageTask("output")
            .AddFileToPackage(@"output\build.exe", "flubu.runner")
            .AddFileToPackage(@"output\flubucore.dll", "flubu.runner")
            .ZipPackage("Flubu runner", true)
            .Execute(context);
    }

    private static void PublishNuGetPackage(ITaskContext context)
    {
        var version = context.Properties.GetBuildVersion();

        try
        {
       
            context.CoreTasks().ExecuteDotnetTask("nuget")
                .WithArguments("push")
                .WithArguments($"output\\FlubuCore.{version.ToString(3)}.nupkg")
                .WithArguments("-s", "https://www.nuget.org/api/v2/package")
                .WithArguments("-k", "8da65a4d-9409-4d1b-9759-3b604d7a34ae").Execute(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to publish FlubuCore. exception: {e.Message}");
        }

        try
        {
            context.CoreTasks().ExecuteDotnetTask("nuget")
           .WithArguments("push")
           .WithArguments($"output\\dotnet-flubu.{version.ToString(3)}.nupkg")
           .WithArguments("-s", "https://www.nuget.org/api/v2/package")
           .WithArguments("-k", "8da65a4d-9409-4d1b-9759-3b604d7a34ae").Execute(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to publish dotnet-flubu. exception: {e.Message}");
        }

        try
        {
            var task = context.Tasks().PublishNuGetPackageTask("FlubuCore.Runner", @"Nuget\FlubuCoreRunner.nuspec");

            task.NuGetServerUrl = "https://www.nuget.org/api/v2/package";
            task.ForApiKeyUse("8da65a4d-9409-4d1b-9759-3b604d7a34ae");
            task.AllowPushOnInteractiveBuild = true;
            task.Execute(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to publish flubu.ruuner. exception: {e}");
        }
    }

    private static void TargetMerge(ITaskContext context)
    {
        var progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net46")
            .WithArguments("inject-dll")
            .WithArguments("--assembly", "dotnet-flubu.exe")
            .WithArguments("--include", "*.dll")
            .WithArguments("--exclude", "FlubuCore.dll")
            .WithArguments("--move")
            .Execute(context);

        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net46\dotnet-flubu.exe", @"output\build.exe", true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net46\FlubuCore.dll", @"output\FlubuCore.dll", true)
            .Execute(context);
        context.Tasks()
           .CopyFileTask(@"dotnet-flubu\bin\Release\net46\FlubuCore.xml", @"output\FlubuCore.xml", true)
           .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net46\FlubuCore.pdb", @"output\FlubuCore.pdb", true)
            .Execute(context);
    }
}