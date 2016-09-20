using flubu.Scripting;
using flubu.Targeting;
using System;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(TaskSession session)
    {
        Console.WriteLine("1");
    }

    protected override void ConfigureTargets(TargetTree targetTree, CommandArguments args)
    {
        Console.WriteLine("2");
    }
}

