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

        public CommandExecutor(IFlubuCommandParser parser)
        {
            _parser = parser;
        }

        public int Execute(string[] args)
        {
            Command commands = _parser.Parse(args);

            return 0;
        }
    }
}