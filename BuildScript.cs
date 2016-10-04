using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(ITaskSession session)
    {
        session.Properties.Set(BuildProps.CompanyName, "Flubu");
        session.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2016 Flubu");
        session.Properties.Set(BuildProps.ProductId, "FlubuCore");
        session.Properties.Set(BuildProps.ProductName, "FlubuCore");
    }

    protected override void ConfigureTargets(ITaskSession session)
    {
        session
            .CreateTarget("compile")
            .AddTask(new FetchVersionFromExternalSourceTask())
            .AddTask(new UpdateNetCoreVersionTask("FlubuCore/project.json", "dotnet-flubu/project.json",
                    "Flubu.Tests/project.json")
                .AdditionalProp("dependencies.FlubuCore", "dependencies.dotnet-flubu"))
            .AddTask(new ExecuteDotnetTask("restore").WithArguments("FlubuCore"))
            .AddTask(new ExecuteDotnetTask("restore").WithArguments("dotnet-flubu"))
            .AddTask(new ExecuteDotnetTask("restore").WithArguments("Flubu.Tests"))
            .AddTask(new ExecuteDotnetTask("pack").WithArguments("FlubuCore", "-c", "Release"))
            .AddTask(new ExecuteDotnetTask("pack").WithArguments("dotnet-flubu", "-c", "Release"));

        session
            .CreateTarget("test")
            .DotnetUnitTest("Flubu.Tests");
    }
}
