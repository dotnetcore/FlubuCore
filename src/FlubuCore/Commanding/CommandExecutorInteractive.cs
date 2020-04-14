using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutorInteractive : ICommandExecutor
    {
        private readonly IFlubuSession _flubuSession;

        private readonly IScriptProperties _scriptProperties;

        private readonly ILogger<CommandExecutorInteractive> _log;

        private readonly IScriptProvider _scriptProvider;

        private CommandArguments _args;

        public CommandExecutorInteractive(
            CommandArguments args,
            IScriptProvider scriptProvider,
            IFlubuSession flubuSession,
            IScriptProperties scriptProperties,
            ILogger<CommandExecutorInteractive> log)
        {
            _args = args;
            _scriptProvider = scriptProvider;
            _flubuSession = flubuSession;
            _scriptProperties = scriptProperties;
            _log = log;
        }

        public virtual async Task<int> ExecuteAsync()
        {
            if (_args.DisableColoredLogging)
            {
                FlubuConsoleLogger.DisableColloredLogging = true;
            }

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
                            script = await _scriptProvider.GetBuildScriptAsync(_args);
                        }
                        else
                        {
                            script = await _scriptProvider.GetBuildScriptAsync(_flubuSession
                                .InteractiveArgs, true);
                        }
                    }
                    catch (BuildScriptLocatorException)
                    {
                        if (!_args.InteractiveMode && !_flubuSession.InteractiveMode)
                        {
                            throw;
                        }

                        _flubuSession.LogInfo("Build script not found.");

                        script = await SimpleFlubuInteractiveMode(script);
                    }

                    _flubuSession.ScriptArgs = _args.ScriptArguments;
                    _flubuSession.InteractiveMode = _args.InteractiveMode;
                    _flubuSession.InteractiveArgs = _args;
                    _flubuSession.TargetTree.BuildScript = script;
                    _flubuSession.Properties.Set(BuildProps.IsWebApi, _args.IsWebApi);
                    _flubuSession.TargetTree.ResetTargetTree();

                    if (script != null)
                    {
                        if (_flubuSession.InteractiveMode)
                        {
                            FlubuInteractiveMode(_flubuSession, script);
                        }
                        else
                        {
                            result = script.Run(_flubuSession);
                        }
                    }
                }
                while (_flubuSession.InteractiveMode && InternalCommands.ReloadCommands.Contains(_flubuSession.InteractiveArgs.MainCommands[0], StringComparer.OrdinalIgnoreCase));

                return result;
            }
            catch (TaskExecutionException e)
            {
                if (_args.RethrowOnException)
                    throw;

                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e}", null, (t, ex) => t);
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

        protected virtual void FlubuInteractiveMode(IFlubuSession flubuSession, IBuildScript script)
         {
            flubuSession.InteractiveMode = true;
            var flubuConsole = InitializeFlubuConsole(flubuSession, script);
            flubuSession.TargetTree.ScriptArgsHelp = _scriptProperties.GetPropertiesHelp(script);
            flubuSession.TargetTree.RunTarget(flubuSession, "help.onlyTargets");
            flubuSession.LogInfo(" ");

            do
            {
                if (flubuSession.InteractiveMode)
                {
                    var commandLine = flubuConsole.ReadLine(Directory.GetCurrentDirectory());

                    if (string.IsNullOrEmpty(commandLine))
                    {
                        continue;
                    }

                    var cmdApp = new CommandLineApplication()
                    {
                        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
                    };
                    var parser = new FlubuCommandParser(cmdApp, null, null, null);
                    var args = parser.Parse(commandLine.Split(' ')

                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim()).ToArray());
                    var targetsInfo = DefaultBuildScript.ParseCmdLineArgs(args.MainCommands, flubuSession.TargetTree);

                    if (args.MainCommands.Count == 0)
                    {
                        continue;
                    }

                    if (!args.MainCommands[0].Equals("l", StringComparison.OrdinalIgnoreCase) &&
                        !args.MainCommands[0].Equals("load", StringComparison.OrdinalIgnoreCase))
                    {
                        args.Script = flubuSession.InteractiveArgs.Script;
                    }

                    flubuSession.InteractiveArgs = args;
                    flubuSession.ScriptArgs = args.ScriptArguments;
                    _scriptProperties.InjectProperties(script, flubuSession);

                    if (InternalCommands.InteractiveExitAndReloadCommands.Contains(args.MainCommands[0],
                        StringComparer.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (targetsInfo.unknownTarget)
                    {
                        var internalCommandExecuted = flubuConsole.ExecuteInternalCommand(commandLine);
                        if (internalCommandExecuted)
                        {
                            continue;
                        }

                        var splitedLine = commandLine.Split(' ').ToList();
                        var command = splitedLine.First();
                        var runProgram = flubuSession.Tasks().RunProgramTask(command).DoNotLogTaskExecutionInfo()
                            .WorkingFolder(Directory.GetCurrentDirectory());
                        splitedLine.RemoveAt(0);
                        try
                        {
                            runProgram.WithArguments(splitedLine.ToArray()).Execute(flubuSession);
                        }
                        catch (CommandUnknownException)
                        {
                            flubuSession.LogError(
                                $"'{command}' is not recognized as a flubu target, internal or external command, operable program or batch file.");
                        }
                        catch (TaskExecutionException e)
                        {
                            flubuSession.LogError($"ERROR: {(flubuSession.Args.Debug ? e.ToString() : e.Message)}", Color.Red);
                        }
                        catch (ArgumentException)
                        {
                        }
                        catch (Win32Exception)
                        {
                        }

                        continue;
                    }
                }

                try
                {
                    script.Run(flubuSession);
                    flubuSession.LogInfo(" ");
                }
                catch (Exception e)
                {
                     flubuSession.LogError($"ERROR: {(flubuSession.Args.Debug ? e.ToString() : e.Message)}", Color.Red);
                }
            }
            while (flubuSession.InteractiveMode);
        }

        protected virtual async Task<IBuildScript> SimpleFlubuInteractiveMode(IBuildScript script)
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

                    if (InternalCommands.InteractiveExitOnlyCommands.Contains(splitedLine[0], StringComparer.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    var cmdApp = new CommandLineApplication()
                    {
                        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
                    };
                    var parser = new FlubuCommandParser(cmdApp, null, null, null);
                    var args = parser.Parse(commandLine.Split(' ')

                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim()).ToArray());
                    _flubuSession.InteractiveArgs = args;
                    _flubuSession.ScriptArgs = args.ScriptArguments;
                    _args = args;
                    _args.InteractiveMode = true;

                    var internalCommandExecuted = flubuConsole.ExecuteInternalCommand(commandLine);
                    if (internalCommandExecuted)
                    {
                        continue;
                    }

                    if (!InternalCommands.ReloadCommands.Contains(splitedLine[0], StringComparer.OrdinalIgnoreCase))
                    {
                        var command = splitedLine.First();
                        var runProgram = _flubuSession.Tasks().RunProgramTask(command).DoNotLogTaskExecutionInfo()
                            .WorkingFolder(Directory.GetCurrentDirectory());
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
                        script = await _scriptProvider.GetBuildScriptAsync(_flubuSession.InteractiveArgs, true);
                    }
                }
                catch (BuildScriptLocatorException)
                {
                    _flubuSession.LogInfo("Build script not found.");
                }
            }
            while (script == null);

            return script;
        }

        protected virtual FlubuConsole InitializeFlubuConsole(IFlubuSession flubuSession, IBuildScript script)
        {
            var source = new Dictionary<string, IReadOnlyCollection<Hint>>();
            var propertyKeys = _scriptProperties.GetPropertiesHints(script);
            propertyKeys.Add(new Hint { Name = "--parallel", Help = "If applied target's are executed in parallel.", HintColor = ConsoleColor.Magenta });
            propertyKeys.Add(new Hint { Name = "--dryrun", Help = "Performs a dry run of the specified target.", HintColor = ConsoleColor.Magenta });
            propertyKeys.Add(new Hint { Name = "--noColor", Help = "Disables colored logging.", HintColor = ConsoleColor.Magenta });
            propertyKeys.Add(new Hint { Name = "--nodeps", Help = "If applied no target dependencies are executed.", HintColor = ConsoleColor.Magenta });
            source.Add("-", propertyKeys);
            var enumHints = _scriptProperties.GetEnumHints(script, flubuSession);
            if (enumHints != null)
            {
                source.AddRange(enumHints);
            }

            List<Hint> defaultHints = new List<Hint>();

            if (script is DefaultBuildScript defaultBuildScript)
            {
                defaultBuildScript.ConfigureDefaultProps(flubuSession);
                defaultBuildScript.ConfigureBuildPropertiesInternal(flubuSession);
                defaultBuildScript.ConfigureTargetsInternal(flubuSession);
            }

            foreach (var targetName in flubuSession.TargetTree.GetTargetNames())
            {
                var target = flubuSession.TargetTree.GetTarget(targetName);
                defaultHints.Add(new Hint
                {
                    Name = target.TargetName,
                    Help = target.Description
                });
            }

            var flubuConsole = new FlubuConsole(flubuSession.TargetTree, defaultHints, source);
            return flubuConsole;
        }
    }
}
