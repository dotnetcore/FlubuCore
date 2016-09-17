namespace flubu.Scripting
{
    public interface IBuildScript
    {
        int Run(CommandArguments args);

        string Name { get; }
    }
}