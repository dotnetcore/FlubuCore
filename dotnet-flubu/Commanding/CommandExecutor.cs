using flubu.Scripting;
using System.Collections.Generic;

namespace flubu.Commanding
{
    public interface ICommandExecutor
    {
        int Execute(string[] args);
    }

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IFlubuCommandParser _parser;
        private readonly IScriptLoader _scriptLoader;

        public CommandExecutor(IFlubuCommandParser parser, IScriptLoader scriptLoader)
        {
            _parser = parser;
            _scriptLoader = scriptLoader;
        }

        public int Execute(string[] args)
        {
            CommandArguments commands = _parser.Parse(args);

            if (commands.Help)
            {
                return 1;
            }

            return 0;
        }
    }
}