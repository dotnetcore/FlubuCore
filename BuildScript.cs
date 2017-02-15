using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Nuget;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.CompanyName, "Flubu");
        context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2016 Flubu");
        context.Properties.Set(BuildProps.ProductId, "FlubuCore");
        context.Properties.Set(BuildProps.ProductName, "FlubuCore");
    }

    protected override void ConfigureTargets(ITaskContext context)
    {
        var buildVersion = context.CreateTarget("buildVersion")
            .AddTask(x => x.FetchBuildVersionFromFileTask());

        var compile =  context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "Flubu.Tests/Flubu.Tests.csproj")
            .AdditionalProp("dependencies.FlubuCore", "dependencies.dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("FlubuCore"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("Flubu.Tests"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack").WithArguments("FlubuCore", "-c", "Release").WithArguments("-o", "..\\build"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack").WithArguments("dotnet-flubu", "-c", "Release").WithArguments("-o", "..\\build"))
            .DependsOn(buildVersion);
        
        context.CreateTarget("merge")
            .SetDescription("Merge's all assemblyes into .net flubu console application")
            .Do(TargetMerge);

        var flubuTests = context.CreateTarget("test")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.ExecuteDotnetTask("test").WithArguments("Flubu.Tests\\Flubu.Tests.csproj"));

        var nuget = context.CreateTarget("nuget.publish")
            .Do(PublishNuGetPackage).
            DependsOn(buildVersion);

        context.CreateTarget("rebuild")
            .SetAsDefault()
            .DependsOn(compile, flubuTests);

        context.CreateTarget("rebuild.server")
           .SetAsDefault()
           .DependsOn(compile, flubuTests, nuget);
    }

    private static void PublishNuGetPackage(ITaskContext context)
    {
        var version = context.Properties.GetBuildVersion();
        context.CoreTasks().ExecuteDotnetTask("nuget")
            .WithArguments("push")
            .WithArguments($"build\\FlubuCore.{version.ToString(3)}.nupkg")
            .WithArguments("-s", "https://www.myget.org/F/flubucore/api/v2/package")
            .WithArguments("-k", "f92a7c72-08b2-4631-af9d-fa2f031eaf8c").Execute(context);

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .WithArguments("push")
            .WithArguments($"build\\dotnet-flubu.{version.ToString(3)}.nupkg")
            .WithArguments("-s", "https://www.myget.org/F/flubucore/api/v2/package")
            .WithArguments("-k", "f92a7c72-08b2-4631-af9d-fa2f031eaf8c").Execute(context);
    }

    private static void TargetMerge(ITaskContext context)
    {
        var progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Debug\net46")
            .WithArguments("inject-dll")
            .WithArguments("--assembly")
            .WithArguments("dotnet-flubu.exe")
            .WithArguments("--include")
            .WithArguments("*.dll")
            .WithArguments("--exclude")
            .WithArguments("FlubuCore.dll")
            .WithArguments("--move")
            .Execute(context);
    }
}