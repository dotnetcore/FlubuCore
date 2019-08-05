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
        private static readonly List<string> _interactiveExitOnlyCommands = new List<string>()
        {
            "x",
            "exit",
            "q",
            "quit",
        };

        private readonly CommandArguments _args;

        private readonly IScriptLoader _scriptLoader;
        private readonly IFlubuSession _flubuSession;
        private readonly IFileWrapper _file;
        private readonly IPathWrapper _path;

        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IScriptLoader scriptLoader,
            IFlubuSession flubuSession,
            IFileWrapper file,
            IPathWrapper path,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _scriptLoader = scriptLoader;
            _flubuSession = flubuSession;
            _file = file;
            _path = path;
            _log = log;
        }

        public static List<string> ReloadCommands => new List<string>
        {
            "r",
            "reload",
            "l",
            "load",
        };

        public static List<string> InteractiveExitCommands
        {
            get
            {
                var ret = new List<string>();
                ret.AddRange(_interactiveExitOnlyCommands);
                ret.AddRange(ReloadCommands);
                return ret;
            }
        }

        public string FlubuHelpText { get; set; }

        public async Task<int> ExecuteAsync()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            if (_args.DisableColoredLogging)
            {
                FlubuConsoleLogger.DisableColloredLogging = true;
            }

            _log.LogInformation($"Flubu v.{version}");

            if (_args.Help) return 1;

            if (_args.MainCommands.Count == 1 && _args.MainCommands.First().Equals("setup", StringComparison.OrdinalIgnoreCase))
            {
                TargetTree.SetupFlubu();
                return 0;
            }

            try
            {
                int result;
                do
                {
                    IBuildScript script;
                    if (!_flubuSession.InteractiveMode)
                    {
                       script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_args);
                    }
                    else
                    {
                        script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_flubuSession.InteractiveArgs);
                    }

                    _flubuSession.FlubuHelpText = FlubuHelpText;
                    _flubuSession.ScriptArgs = _args.ScriptArguments;
                    _flubuSession.TargetTree.ResetTargetTree();
                    result = script.Run(_flubuSession);
                }
                while (_flubuSession.InteractiveMode && ReloadCommands.Contains(_flubuSession.InteractiveArgs.MainCommands[0], StringComparer.OrdinalIgnoreCase));

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