using System;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(ITaskSession session)
    {
        Console.WriteLine("1");
    }

    protected override void ConfigureTargets(ITaskSession session)
    {
        session
            .CreateTarget("compile")
            //.AddTask(new FetchVersionFromExternalSourceTask())
            .AddTask(new UpdateNetCoreVersionTask("FlubuCore/project.json", "dotnet-flubu/project.json",
                    "Flubu.Tests/project.json")
                .FixedVersion(new Version(1, 0, 82, 0))
                .AdditionalProp("dependencies.FlubuCore", "dependencies.dotnet-flubu"))
            .AddTask(new ExecuteDotnetTask("restore").WithArguments("FlubuCore"))
            .AddTask(new ExecuteDotnetTask("restore").WithArguments("dotnet-flubu"))
            .AddTask(new ExecuteDotnetTask("restore").WithArguments("Flubu.Tests"))
            .AddTask(new ExecuteDotnetTask("pack").WithArguments("FlubuCore", "-c", "Release"))
            .AddTask(new ExecuteDotnetTask("pack").WithArguments("dotnet-flubu", "-c", "Release"));
    }
}
