using System.Threading.Tasks;
using Flubu.Scripting;
using Microsoft.Extensions.Logging;

namespace Flubu.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IBuildScriptLocator _locator;

        private readonly ILogger<CommandExecutor> _log;

        private readonly IFlubuCommandParser _parser;

        public CommandExecutor(
            IFlubuCommandParser parser,
            IBuildScriptLocator locator,
            ILogger<CommandExecutor> log)
        {
            _parser = parser;
            _locator = locator;
            _log = log;
        }

        public async Task<int> Execute(string[] args)
        {
            var commands = _parser.Parse(args);

            if (commands.Help)
            {
                return 1;
            }

            var script = await _locator.FindBuildScript(commands);

            if (script == null)
            {
                _log.LogInformation("Script not found!");
                return -1;
            }

            return script.Run(commands);
        }
    }
}