using FlubuCore.Scripting;

namespace DotNet.Cli.Flubu.Commanding
{
    public interface IFlubuCommandParser
    {
        CommandArguments Parse(string[] args);

        void ShowHelp();
    }
}
