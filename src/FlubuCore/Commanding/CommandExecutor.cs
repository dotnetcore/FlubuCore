using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Commanding.Internal;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Templating;
using FlubuCore.Templating.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutor : InternalCommandExecutor, ICommandExecutor
    {
        private readonly IScriptProvider _scriptProvider;

        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IScriptProvider scriptProvider,
            IFlubuSession flubuSession,
            ILogger<CommandExecutor> log,
            IFlubuTemplateTasksExecutor flubuTemplateTasksExecutor)
            : base(flubuSession, args, flubuTemplateTasksExecutor)
        {
            _scriptProvider = scriptProvider;
            _log = log;
        }

        public virtual async Task<int> ExecuteAsync()
        {
            if (Args.DisableColoredLogging)
            {
                FlubuConsoleLogger.DisableColloredLogging = true;
            }

            if (Args.Help) return 1;

            if (Args.IsInternalCommand())
            {
                await ExecuteInternalCommand();
                return 0;
            }

            try
            {
                    var script = await _scriptProvider.GetBuildScriptAsync(Args);
                    FlubuSession.ScriptArgs = Args.ScriptArguments;
                    FlubuSession.TargetTree.BuildScript = script;
                    FlubuSession.Properties.Set(BuildProps.IsWebApi, Args.IsWebApi);
                    var result = script.Run(FlubuSession);
                    return result;
            }
            catch (TaskExecutionException e)
            {
                if (Args.RethrowOnException)
                    throw;

                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e.ToString()}", null, (t, ex) => t);
                return StatusCodes.BuildScriptNotFound;
            }
            catch (FlubuException e)
            {
                if (Args.RethrowOnException)
                    throw;

                var str = Args.Debug ? e.ToString() : e.Message;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                return StatusCodes.BuildScriptNotFound;
            }
            catch (Exception e)
            {
                if (Args.RethrowOnException)
                    throw;

                var str = Args.Debug ? e.ToString() : e.Message;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                return 3;
            }
        }
    }
}