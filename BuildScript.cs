using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;
using System.IO;

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
        context
            .CreateTarget("compile")
            .AddTask(x => x.FetchVersionFromExternalSourceTask())
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/project.json", "dotnet-flubu/project.json","Flubu.Tests/project.json")
                .AdditionalProp("dependencies.FlubuCore", "dependencies.dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("FlubuCore"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("Flubu.Tests"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack").WithArguments("FlubuCore", "-c", "Release"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack").WithArguments("dotnet-flubu", "-c", "Release"));
        
        context.CreateTarget("merge")
            .Do(TargetIlMerge);

        context.CreateTarget("test")
            .TaskExtensions()
            .DotnetUnitTest("Flubu.Tests");
    }

    private static void TargetIlMerge(ITaskContext context)
    {
        var progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");
        
        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Debug\net46\win7-x64")
            .WithArguments("inject-dll")
            .WithArguments("--assembly")
            .WithArguments("dotnet-flubu.exe")
            .WithArguments("--include")
            .WithArguments("*.dll")
            .WithArguments("--move")
            .Execute(context);
    }
}
