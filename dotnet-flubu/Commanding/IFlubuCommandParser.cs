using Flubu.Scripting;

namespace Flubu.Commanding
{
    public interface IFlubuCommandParser
    {
        CommandArguments Parse(string[] args);

        void ShowHelp();
    }
}
