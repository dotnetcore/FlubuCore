using FlubuCore.Scripting;

namespace FlubuCore.Runner.Commanding
{
    public interface IFlubuCommandParser
    {
        CommandArguments Parse(string[] args);

        void ShowHelp();
    }
}
