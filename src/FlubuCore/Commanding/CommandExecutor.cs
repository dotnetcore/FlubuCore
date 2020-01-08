using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DotNet.Cli.Utils;
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

        private readonly IScriptLoader _scriptLoader;

        private readonly IFlubuSession _flubuSession;

        private readonly ILogger<CommandExecutor> _log;

        private CommandArguments _args;

        public CommandExecutor(
            CommandArguments args,
            IScriptLoader scriptLoader,
            IFlubuSession flubuSession,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _scriptLoader = scriptLoader;
            _flubuSession = flubuSession;
            _log = log;
        }

        public static List<string> ReloadCommands => new List<string>
        {
            "r",
            "reload",
            "l",
            "load",
        };

        public static List<string> InteractiveExitAndReloadCommands
        {
            get
            {
                var ret = new List<string>();
                ret.AddRange(ReloadCommands);
                ret.AddRange(_interactiveExitOnlyCommands);
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
                int result = 0;
                do
                {
                    IBuildScript script = null;

                    try
                    {
                        if (!_flubuSession.InteractiveMode)
                        {
                            script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_args);
                        }
                        else
                        {
                            script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_flubuSession
                                .InteractiveArgs);
                        }
                    }
                    catch (BuildScriptLocatorException)
                    {
                        if (!_args.InteractiveMode && !_flubuSession.InteractiveMode)
                        {
                            throw;
                        }

                        script = await SimpleFlubuInteractiveMode(script);
                    }

                    _flubuSession.ScriptArgs = _args.ScriptArguments;
                    _flubuSession.InteractiveArgs = _args;
                    _flubuSession.TargetTree.BuildScript = script;
                    _flubuSession.FlubuHelpText = FlubuHelpText;
                    _flubuSession.Properties.Set(BuildProps.IsWebApi, _args.IsWebApi);
                    _flubuSession.TargetTree.ResetTargetTree();

                    //// ReSharper disable once PossibleNullReferenceException
                    if (script != null)
                    {
                        result = script.Run(_flubuSession);
                    }
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

        private async Task<IBuildScript> SimpleFlubuInteractiveMode(IBuildScript script)
        {
            do
            {
                try
                {
                    var flubuConsole = new FlubuConsole(_flubuSession.TargetTree, new List<Hint>());
                    var commandLine = flubuConsole.ReadLine(Directory.GetCurrentDirectory());

                    if (string.IsNullOrEmpty(commandLine))
                    {
                        continue;
                    }

                    var splitedLine = commandLine.Split(' ').ToList();

                    if (_interactiveExitOnlyCommands.Contains(splitedLine[0], StringComparer.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    var app = new CommandLineApplication(false);
                    IFlubuCommandParser parser = new FlubuCommandParser(app, null);
                    var args = parser.Parse(commandLine.Split(' ')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim()).ToArray());
                    _flubuSession.InteractiveArgs = args;
                    _flubuSession.ScriptArgs = args.ScriptArguments;
                    _args = args;

                    var internalCommandExecuted = flubuConsole.ExecuteInternalCommand(commandLine);
                    if (internalCommandExecuted)
                    {
                        continue;
                    }

                    if (!ReloadCommands.Contains(splitedLine[0], StringComparer.OrdinalIgnoreCase))
                    {
                        var command = splitedLine.First();
                        var runProgram = _flubuSession.Tasks().RunProgramTask(command).DoNotLogTaskExecutionInfo()
                            .WorkingFolder(".");
                        splitedLine.RemoveAt(0);
                        try
                        {
                            runProgram.WithArguments(splitedLine.ToArray()).Execute(_flubuSession);
                        }
                        catch (CommandUnknownException)
                        {
                            _flubuSession.LogError($"'{command}' is not recognized as a internal or external command, operable program or batch file.");
                        }
                        catch (TaskExecutionException)
                        {
                        }
                        catch (ArgumentException)
                        {
                        }
                        catch (Win32Exception)
                        {
                        }
                    }
                    else
                    {
                        script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_flubuSession.InteractiveArgs);
                    }
                }
                catch (BuildScriptLocatorException)
                {
                }
            }
            while (script == null);

            return script;
        }
    }
}