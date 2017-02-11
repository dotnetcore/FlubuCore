using System;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Runner.Scripting;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Runner.Commanding
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

            try
            {
                IBuildScript script = await _locator.FindBuildScript(_args);

                if (script == null)
                {
                    ReportUnspecifiedBuildScript();
                    return -1;
                }

                var result = script.Run(_taskSession);

                return result;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e}", null, (t, ex) => t);
                return 3;
            }
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