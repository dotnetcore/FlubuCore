using flubu.Scripting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flubu.Commanding
{
    public interface ICommandExecutor
    {
        Task<int> Execute(string[] args);
    }

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IFlubuCommandParser _parser;
        private readonly IBuildScriptLocator _locator;
        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(IFlubuCommandParser parser,
            IBuildScriptLocator locator, ILogger<CommandExecutor> log)
        {
            _parser = parser;
            _locator = locator;
            _log = log;
        }

        public async Task<int> Execute(string[] args)
        {
            CommandArguments commands = _parser.Parse(args);

            if (commands.Help)
            {
                return 1;
            }

            IBuildScript script = await _locator.FindBuildScript(commands);

            if (script==null)
            {
                _log.LogInformation("Script not found!");
                return -1;
            }

            return script.Run(commands);
        }
    }
}