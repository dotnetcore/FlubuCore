public class MyBuildScript : flubu.Scripting.IBuildScript
{
    public string Name => nameof(MyBuildScript);

    public int Run(flubu.Scripting.CommandArguments args)
    {
        System.Console.WriteLine("111");
        return 0;
    }
}

