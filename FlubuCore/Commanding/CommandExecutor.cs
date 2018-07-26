using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly CommandArguments _args;

        private readonly IScriptLoader _scriptLoader;
        private readonly ITaskSession _taskSession;

        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IScriptLoader scriptLoader,
            ITaskSession taskSession,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _scriptLoader = scriptLoader;
            _taskSession = taskSession;
            _log = log;
        }

        public string FlubuHelpText { get; set; }

        public async Task<int> ExecuteAsync()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            _log.LogInformation($"Flubu v.{version}");

            if (_args.Help) return 1;

            try
            {
                var script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_args);

                _taskSession.FlubuHelpText = FlubuHelpText;
                _taskSession.ScriptArgs = _args.ScriptArguments;
                var result = script.Run(_taskSession);
                return result;
            }
            catch (FlubuException e)
            {
                if (_args.RethrowOnException)
                    throw;

                var str = _args.Debug ? e.ToString() : e.Message;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                return StatusCodes.BuildScriptNotFound;
            }
            catch (Exception e)
            {
                if (_args.RethrowOnException)
                    throw;

                var str = _args.Debug ? e.ToString() : e.Message;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                return 3;
            }
        }
    }
}