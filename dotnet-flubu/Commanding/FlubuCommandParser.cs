using System.Collections.Generic;
using System.Linq;
using FlubuCore;
using FlubuCore.Scripting;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Cli.Flubu.Commanding
{
    public class FlubuCommandParser : IFlubuCommandParser
    {
        private readonly CommandLineApplication _commandApp;

        private readonly IFlubuConfigurationProvider _flubuConfigurationProvider;

        private CommandArgument _command;

        private CommandOption _configurationOption;

        private CommandOption _outputOption;

        private CommandArguments _parsed;

        private CommandOption _scriptPath;

        private CommandOption _targetsToExecute;

        private CommandOption _parallelTargetExecution;

        private CommandOption _isDebug;

        private CommandOption _configurationFile;

        private CommandOption _assemblyDirectories;

        public FlubuCommandParser(
            CommandLineApplication commandApp,
            IFlubuConfigurationProvider flubuConfigurationProvider)
        {
            _commandApp = commandApp;
            _flubuConfigurationProvider = flubuConfigurationProvider;
        }

        public CommandArguments Parse(string[] args)
        {
            _parsed = new CommandArguments();

            _commandApp.HelpOption("-?|-h|--help");

            _command = _commandApp.Argument("<COMMAND> [arguments]", "The command to execute", true);

            _configurationOption = _commandApp.Option("-c|--configuration <CONFIGURATION>", "Configuration under which to run", CommandOptionType.SingleValue);
            _outputOption = _commandApp.Option("-o|--output <OUTPUT_DIR>", "Directory in which to find the binaries to be run", CommandOptionType.SingleValue);
            _scriptPath = _commandApp.Option("-s|--script <SCRIPT>", "Build script file to use.", CommandOptionType.SingleValue);
            _parallelTargetExecution = _commandApp.Option("--parallel", "If applied target's are executed in parallel", CommandOptionType.NoValue);
            _targetsToExecute = _commandApp.Option("-tte|--targetsToExecute <TARGETS_TO_EXECUTE>", "Target's that must be executed. Otherwise fails.", CommandOptionType.SingleValue);
            _isDebug = _commandApp.Option("-d|--debug", "Enable debug logging.", CommandOptionType.NoValue);
            _configurationFile = _commandApp.Option("-cf|--configurationFile", "Path to the json configuration file. If not specified configuration is readed from flubusettings.json ", CommandOptionType.SingleValue);
            _assemblyDirectories = _commandApp.Option("-ass", "Directory to search assemblies to include in script.", CommandOptionType.MultipleValue);
            _commandApp.OnExecute(() => PrepareDefaultArguments());

            if (args == null)
            {
                args = new string[0];
            }

            _parsed.Help = true;

            _commandApp.Execute(args);

            return _parsed;
        }

        public void ShowHelp() => _commandApp.ShowHelp();

        private int PrepareDefaultArguments()
        {
            _parsed.Help = false;

            _parsed.Output = _outputOption.Value();
            _parsed.Config = _configurationOption.Value() ?? Constants.DefaultConfiguration;
            _parsed.MainCommands = _command.Values;
            _parsed.Script = _scriptPath.Value();
            _parsed.AssemblyDirectories = _assemblyDirectories.Values;
            if (_targetsToExecute.HasValue())
            {
                _parsed.TargetsToExecute = _targetsToExecute.Value().Split(',').ToList();
            }

            if (_parallelTargetExecution.HasValue())
            {
                _parsed.ExecuteTargetsInParallel = true;
            }

            if (_isDebug.HasValue())
                _parsed.Debug = true;

            PrepareRemaingCommandsAndScriptArgs();

            return 0;
        }

        private void PrepareRemaingCommandsAndScriptArgs()
        {
            _parsed.RemainingCommands = new List<string>();
            _parsed.ScriptArguments = new DictionaryWithDefault<string, string>(null);
            foreach (var remainingArgument in _commandApp.RemainingArguments)
            {
                if (remainingArgument.StartsWith("-"))
                {
                    var arg = remainingArgument.TrimStart('-');
                    if (arg.Contains("="))
                    {
                        var splitedArg = arg.Split(new[] { '=' }, 2);
                        _parsed.ScriptArguments.Add(splitedArg[0], splitedArg[1]);
                    }
                    else
                    {
                        _parsed.ScriptArguments.Add(arg, null);
                    }
                }
                else
                {
                    _parsed.RemainingCommands.Add(remainingArgument);
                }
            }

            GetScriptArgumentsFromConfiguration();
        }

        private void GetScriptArgumentsFromConfiguration()
        {
            if (_flubuConfigurationProvider == null)
                return;

            var configurationFile = !string.IsNullOrEmpty(_configurationFile.Value()) ? _configurationFile.Value() : "flubusettings.json";

            var options = _flubuConfigurationProvider.GetConfiguration(configurationFile);
            foreach (var option in options)
            {
                if (!_parsed.ScriptArguments.ContainsKey(option.Key))
                {
                    _parsed.ScriptArguments.Add(option.Key, option.Value);
                }
            }
        }
    }
}