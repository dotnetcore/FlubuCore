using FlubuCore.Context;
using FlubuCore.Scripting;

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
        var fetchVersion =  context.CreateTarget("fetch.build.version")
            .AddTask(x => x.FetchBuildVersionFromFileTask());

        var generateAssInfo = context.CreateTarget("generate.common.assinfo")
            .AddTask(x => x.GenerateCommonAssemblyInfoTask())
            .DependsOn(fetchVersion);

        var compile =  context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            ////.AddTask(x => x.FetchVersionFromExternalSourceTask())
            //// .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/project.json", "dotnet-flubu/project.json","Flubu.Tests/project.json")
            //// .AdditionalProp("dependencies.FlubuCore", "dependencies.dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("FlubuCore"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("dotnet-flubu"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("Flubu.Tests"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack").WithArguments("FlubuCore", "-c", "Release"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack").WithArguments("dotnet-flubu", "-c", "Release"));
        
        context.CreateTarget("merge")
            .SetDescription("Merge's all assemblyes into .net flubu console application")
            .Do(TargetIlMerge);

        var flubuTests = context.CreateTarget("test")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.ExecuteDotnetTask("test").WithArguments("Flubu.Tests\\Flubu.Tests.csproj"));

        context.CreateTarget("rebuild")
            .SetAsDefault()
            .DependsOn("generate.common.assinfo", "compile", "test");
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
