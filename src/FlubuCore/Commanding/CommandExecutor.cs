using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly CommandArguments _args;
        private readonly IScriptProvider _scriptProvider;
        private readonly IFlubuSession _flubuSession;
        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IScriptProvider scriptProvider,
            IFlubuSession flubuSession,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _scriptProvider = scriptProvider;
            _flubuSession = flubuSession;
            _log = log;
        }

        public string FlubuHelpText { get; set; }

        public virtual async Task<int> ExecuteAsync()
        {
            if (_args.DisableColoredLogging)
            {
                FlubuConsoleLogger.DisableColloredLogging = true;
            }

            if (_args.Help) return 1;

            if (_args.IsFlubuSetup())
            {
                TargetTree.SetupFlubu();
                return 0;
            }

            try
            {
                    var script = await _scriptProvider.GetBuildScriptAsync(_args);
                    _flubuSession.FlubuHelpText = FlubuHelpText;
                    _flubuSession.ScriptArgs = _args.ScriptArguments;
                    _flubuSession.TargetTree.BuildScript = script;
                    _flubuSession.Properties.Set(BuildProps.IsWebApi, _args.IsWebApi);
                    var result = script.Run(_flubuSession);
                    return result;
            }
            catch (TaskExecutionException e)
            {
                if (_args.RethrowOnException)
                    throw;

                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e.ToString()}", null, (t, ex) => t);
                return StatusCodes.BuildScriptNotFound;
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