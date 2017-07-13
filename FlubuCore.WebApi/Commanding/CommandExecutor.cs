using System;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.WebApi.Commanding
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
                ReportUnspecifiedBuildScript();
                return -1;
            }

            var result = script.Run(_taskSession);

            return result;
        }

        private static void ReportUnspecifiedBuildScript()
        {
            StringBuilder errorMsg = new StringBuilder("The build script file was not specified. Please specify it as the first argument or use some of the default paths for script file: ");
            foreach (var defaultScriptLocation in BuildScriptLocator.DefaultScriptLocations)
            {
                errorMsg.AppendLine(defaultScriptLocation);
            }

            throw new BuildScriptLocatorException(errorMsg.ToString());
        }
    }
}