public class MyBuildScript1 : flubu.Scripting.DefaultBuildScript
{
    protected override void ConfigureBuildProperties(flubu.TaskSession session)
    {
        System.Console.WriteLine("2222");
        System.Diagnostics.Debug.WriteLine("2222");
    }

    protected override void ConfigureTargets(flubu.Targeting.TargetTree targetTree, flubu.Scripting.CommandArguments args)
    {
        System.Console.WriteLine("2222");
        System.Diagnostics.Debug.WriteLine("2222");
    }
}

