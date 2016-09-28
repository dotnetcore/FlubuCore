using System;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(ITaskSession session)
    {
    }

    protected override void ConfigureTargets(ITaskSession session)
    {
        Target
            .Create("compile")
            .AddTask(new ExecuteDotnetTask("build").WithArguments("FlubuCore"))
            .AddToTargetTree(session.TargetTree);
    }
}
