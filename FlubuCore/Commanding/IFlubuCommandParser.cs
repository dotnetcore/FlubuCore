using FlubuCore.Scripting;

namespace FlubuCore.Commanding
{
    public interface IFlubuCommandParser
    {
        CommandArguments Parse(string[] args);

        void ShowHelp();
    }
}
