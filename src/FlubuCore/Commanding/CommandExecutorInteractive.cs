using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlubuCore.Commanding.Internal;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Templating.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutorInteractive : InternalCommandExecutor, ICommandExecutor
    {
        private readonly IFlubuSession _flubuSession;

        private readonly ITargetCreator _targetCreator;

        private readonly IScriptProperties _scriptProperties;

        private readonly ILogger<CommandExecutorInteractive> _log;

        private readonly IScriptProvider _scriptProvider;

        public CommandExecutorInteractive(
            CommandArguments args,
            IScriptProvider scriptProvider,
            IFlubuSession flubuSession,
            ITargetCreator targetCreator,
            IScriptProperties scriptProperties,
            ILogger<CommandExecutorInteractive> log,
            IFlubuTemplateTasksExecutor flubuTemplateTasksExecutor)
            : base(flubuSession, args, flubuTemplateTasksExecutor)
        {
            _scriptProvider = scriptProvider;
            _flubuSession = flubuSession;
            _targetCreator = targetCreator;
            _scriptProperties = scriptProperties;
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
                int result = 0;
                do
                {
                    IBuildScript script = null;

                    try
                    {
                        if (!_flubuSession.InteractiveMode)
                        {
                            script = await _scriptProvider.GetBuildScriptAsync(Args);
                        }
                        else
                        {
                            script = await _scriptProvider.GetBuildScriptAsync(_flubuSession
                                .InteractiveArgs, true);
                        }
                    }
                    catch (BuildScriptLocatorException)
                    {
                        if (Args.InteractiveMode && !_flubuSession.InteractiveMode)
                        {
                            _flubuSession.LogInfo("Build script not found.");
                            script = await SimpleFlubuInteractiveMode(script);
                        }
                        else
                        {
                            throw;
                        }
                    }

                    _flubuSession.ScriptArgs = Args.ScriptArguments;
                    _flubuSession.InteractiveMode = Args.InteractiveMode;
                    _flubuSession.InteractiveArgs = Args;
                    _flubuSession.TargetTree.BuildScript = script;
                    _flubuSession.Properties.Set(BuildProps.IsWebApi, Args.IsWebApi);
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
                while (_flubuSession.InteractiveMode && InternalTerminalCommands.ReloadCommands.Contains(_flubuSession.InteractiveArgs.MainCommands[0], StringComparer.OrdinalIgnoreCase));

                return result;
            }
            catch (TaskExecutionException e)
            {
                if (Args.RethrowOnException)
                    throw;

                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e}", null, (t, ex) => t);
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

        protected virtual void FlubuInteractiveMode(IFlubuSession flubuSession, IBuildScript script)
         {
             _targetCreator.CreateTargetFromMethodAttributes(script, flubuSession);
             _scriptProperties.InjectProperties(script, flubuSession);
            flubuSession.InteractiveMode = true;
            var flubuConsole = InitializeFlubuConsole(flubuSession, script);
            flubuSession.TargetTree.ScriptArgsHelp = _scriptProperties.GetPropertiesHelp(script);
            flubuSession.TargetTree.RunTarget(flubuSession, FlubuTargets.HelpOnlyTargets);
            flubuSession.LogInfo(" ");

            do
            {
                if (flubuSession.InteractiveMode)
                {
                    var commandLine = flubuConsole.ReadLine();

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

                    if (InternalTerminalCommands.InteractiveExitAndReloadCommands.Contains(args.MainCommands[0],
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
                    var commandLine = flubuConsole.ReadLine();

                    if (string.IsNullOrEmpty(commandLine))
                    {
                        continue;
                    }

                    var splitedLine = commandLine.Split(' ').ToList();

                    if (InternalTerminalCommands.InteractiveExitOnlyCommands.Contains(splitedLine[0], StringComparer.OrdinalIgnoreCase))
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
                    Args = args;
                    Args.InteractiveMode = true;

                    var internalCommandExecuted = flubuConsole.ExecuteInternalCommand(commandLine);
                    if (internalCommandExecuted)
                    {
                        continue;
                    }

                    if (!InternalTerminalCommands.ReloadCommands.Contains(splitedLine[0], StringComparer.OrdinalIgnoreCase))
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
