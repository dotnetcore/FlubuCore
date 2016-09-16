public class MyBuildScript : flubu.Scripting.IBuildScript
{
    public string Name => nameof(MyBuildScript);

    public int Run(string[] args)
    {
        System.Console.WriteLine("Executed");
        return 0;
    }
}

