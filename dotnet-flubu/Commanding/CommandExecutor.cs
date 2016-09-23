using System.Threading.Tasks;
using Flubu.Scripting;
using Flubu.Tasks;
using Microsoft.Extensions.Logging;

namespace Flubu.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly CommandArguments _args;
        private readonly IBuildScriptLocator _locator;
        private readonly ITaskSession _taskSession;

        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IBuildScriptLocator locator,
            ITaskSession taskSession,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _locator = locator;
            _taskSession = taskSession;
            _log = log;
        }

        public async Task<int> ExecuteAsync()
        {
            if (_args.Help)
            {
                return 1;
            }

            IBuildScript script = await _locator.FindBuildScript(_args);

            if (script == null)
            {
                _log.LogInformation("Script not found!");
                return -1;
            }

            return script.Run(_taskSession);
        }
    }
}