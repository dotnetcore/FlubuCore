using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using McMaster.Extensions.CommandLineUtils;

namespace FlubuCore.Commanding
{
    public class FlubuCommandParser : IFlubuCommandParser
    {
        private static readonly List<string> DefaultSettingLocations = new List<string>
        {
            "flubusettings.json",
            "_Build/flubusettings.json",
            "build/flubusettings.json",
            "Build/flubusettings.json",
            "_BuildScript/flubusettings.json",
            "_BuildScripts/flubusettings.json",
            "BuildScript/flubusettings.json",
            "BuildScripts/flubusettings.json",
        };

        private readonly CommandLineApplication _commandApp;

        private readonly IFlubuConfigurationProvider _flubuConfigurationProvider;

        private readonly IBuildScriptLocator _buildScriptLocator;

        private readonly IFileWrapper _file;

        private CommandArgument _command;

        private CommandArguments _parsed;

        private CommandOption _scriptPath;

        private CommandOption _targetsToExecute;

        private CommandOption _parallelTargetExecution;

        private CommandOption _isDebug;

        private CommandOption _configurationFile;

        private CommandOption _assemblyDirectories;

        private CommandOption _noDependencies;

        private CommandOption _dryRun;

        private CommandOption _noInteractive;

        private CommandOption _noColor;

        private CommandOption _interactiveMode;

        private CommandOption _generateContinousIntegrationConfigs;

        public FlubuCommandParser(
            CommandLineApplication commandApp,
            IFlubuConfigurationProvider flubuConfigurationProvider,
            IBuildScriptLocator buildScriptLocator,
            IFileWrapper file)
        {
            _commandApp = commandApp;
            _flubuConfigurationProvider = flubuConfigurationProvider;
            _buildScriptLocator = buildScriptLocator;
            _file = file;
        }

        public virtual CommandArguments Parse(string[] args)
        {
            _parsed = new CommandArguments();

            _commandApp.HelpOption("-?|-h|--help");
#if NET462
            _commandApp.Name = "flubu.exe";
#else
            _commandApp.Name = "flubu";
#endif
            _command = _commandApp.Argument("<Target> [build script arguments]", "The target to execute.", true);
            _scriptPath = _commandApp.Option("-s|--script <SCRIPT>", "Build script file to use.", CommandOptionType.SingleValue);
            _interactiveMode = _commandApp.Option("-i|--interactive", "Run's Flubu core in interactive mode", CommandOptionType.NoValue);
            _parallelTargetExecution = _commandApp.Option("--parallel", "If applied target's are executed in parallel", CommandOptionType.NoValue);
            _targetsToExecute = _commandApp.Option("-tte|--targetsToExecute <TARGETS_TO_EXECUTE>", "Target's that must be executed. Otherwise fails.", CommandOptionType.SingleValue);
            _isDebug = _commandApp.Option("-d|--debug", "Enable debug logging.", CommandOptionType.NoValue);
            _configurationFile = _commandApp.Option("-cf|--configurationFile", $"Path to the Flubu json configuration file. If not specified configuration is read from flubusettings.json {Environment.NewLine}", CommandOptionType.SingleValue);
            _assemblyDirectories = _commandApp.Option("-ass", $"Directory to search assemblies to include automatically in script (Assemblies in subdirectories are also loaded). If not specified assemblies are loaded by default from FlubuLib directory. {Environment.NewLine}", CommandOptionType.MultipleValue);
            _noDependencies = _commandApp.Option("-nd||--nodeps", "If applied no target dependencies are executed.", CommandOptionType.NoValue);
            _dryRun = _commandApp.Option("--dryRun", "Performs a dry run of the specified target.", CommandOptionType.NoValue);
            _noInteractive = _commandApp.Option("--noint", $"Disables interactive mode for all task members. Default values are used instead. {Environment.NewLine}", CommandOptionType.NoValue);
            _noColor = _commandApp.Option("--noColor", "Disables colored logging", CommandOptionType.NoValue);
            _generateContinousIntegrationConfigs = _commandApp.Option("--ci", "Generates configuration file for specified continous integration server. Supported values: Jenkins, AppVeyor, Travis, AzurePipelines, GithubActions", CommandOptionType.MultipleValue);
            _commandApp.ExtendedHelpText = @"
Flubu internal commands:
  <Target> help                                 Shows detailed help for specified target.
  new                                           Creates new build script project from template. Execute 'flubu new' for list of templates.
  setup                                         Store locations of build script, project file or flubu settings file into '.flubu' file.
                                                You should execute this command at the root of your repository.";

            _commandApp.OnExecute(() => PrepareDefaultArguments());

            if (args == null)
            {
                args = new string[0];
            }

            _parsed.Help = true;

            _commandApp.Execute(args);

            _parsed.FlubuHelpText = _commandApp.GetHelpText();

            if (_buildScriptLocator != null)
            {
                _parsed.FlubuFileLocation = _buildScriptLocator.FindFlubuFile();
            }

            return _parsed;
        }

        public void ShowHelp() => _commandApp.ShowHelp();

        private int PrepareDefaultArguments()
        {
            _parsed.Help = false;
            _parsed.MainCommands = _command.Values.Where(x => !x.StartsWith("/")).ToList();
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

            if (_noDependencies.HasValue())
            {
                _parsed.NoDependencies = true;
            }

            if (_dryRun.HasValue())
            {
                _parsed.DryRun = true;
            }

            if (_noInteractive.HasValue())
            {
                _parsed.DisableInteractive = true;
            }

            if (_noColor.HasValue())
            {
                _parsed.DisableColoredLogging = true;
            }

            if (_interactiveMode.HasValue())
            {
                _parsed.InteractiveMode = true;
            }

            if (_isDebug.HasValue())
                _parsed.Debug = true;

            if (_generateContinousIntegrationConfigs.HasValue())
            {
                _parsed.GenerateContinousIntegrationConfigs = new List<BuildServerType>();
                foreach (var value in _generateContinousIntegrationConfigs.Values)
                {
                    switch (value.ToLower())
                    {
                        case "jenkins":
                        {
                            _parsed.GenerateContinousIntegrationConfigs.Add(BuildServerType.Jenkins);
                            break;
                        }

                        case "appveyor":
                        {
                            _parsed.GenerateContinousIntegrationConfigs.Add(BuildServerType.AppVeyor);
                            break;
                        }

                        case "travis":
                        case "travisci":
                        {
                            _parsed.GenerateContinousIntegrationConfigs.Add(BuildServerType.TravisCI);
                            break;
                        }

                        case "azurepipelines":
                        case "azurepipeline":
                        case "azure":
                        {
                            _parsed.GenerateContinousIntegrationConfigs.Add(BuildServerType.AzurePipelines);
                            break;
                        }

                        case "githubactions":
                        case "githubaction":
                        case "github":
                        case "actions":
                        case "workflow":
                        case "ghworkflow":
                        {
                            _parsed.GenerateContinousIntegrationConfigs.Add(BuildServerType.GitHubActions);
                            break;
                        }
                    }
                }

                _parsed.GenerateContinousIntegrationConfigs = _parsed.GenerateContinousIntegrationConfigs.Distinct().ToList();
            }

            PrepareRemaingCommandsAndScriptArgs();

            return 0;
        }

        private void PrepareRemaingCommandsAndScriptArgs()
        {
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
                    _parsed.AdditionalOptions.Add(remainingArgument);
                }
            }

            _parsed.AdditionalOptions.AddRange(_command.Values.Where(x => x.StartsWith("/")).ToList());

            GetScriptArgumentsFromConfiguration();
        }

        private void GetScriptArgumentsFromConfiguration()
        {
            if (_flubuConfigurationProvider == null)
                return;

            var configurationFile = !string.IsNullOrEmpty(_configurationFile.Value()) ? _configurationFile.Value() : GetConfigurationFile();

            var options = _flubuConfigurationProvider.GetConfiguration(configurationFile);
            if (options == null)
            {
                return;
            }

            foreach (var option in options)
            {
                var key = option.Key.ToLowerInvariant();
                switch (key)
                {
                    case "s":
                    case "script":
                    {
                        if (string.IsNullOrEmpty(_parsed.Script))
                        {
                            _parsed.Script = option.Value;
                        }

                        break;
                    }

                    case "d":
                    case "debug":
                    {
                        if (!_parsed.Debug)
                        {
                            bool result;
                            if (bool.TryParse(option.Value, out result))
                            {
                                if (result)
                                {
                                    _parsed.Debug = true;
                                }
                            }
                        }

                        break;
                    }

                    case "ass":
                    {
                        _parsed.AssemblyDirectories.Add(option.Value);
                        break;
                    }

                    case "noColor":
                    {
                        _parsed.DisableColoredLogging = true;
                        break;
                    }

                    default:
                    {
                        if (!_parsed.ScriptArguments.ContainsKey(option.Key))
                        {
                            _parsed.ScriptArguments.Add(option.Key, option.Value);
                        }

                        break;
                    }
                }
            }
        }

        private string GetConfigurationFile()
        {
            string flubuConfigurationFile = GetConfigurationFileFromFlubuFile();

            if (flubuConfigurationFile != null)
            {
                return flubuConfigurationFile;
            }

            return GetConfigurationFileFromDefaultLocations();
        }

        private string GetConfigurationFileFromFlubuFile()
        {
            string flubuFile = _buildScriptLocator.FindFlubuFile();

            if (string.IsNullOrEmpty(flubuFile))
            {
                return null;
            }

            var lines = _file.ReadAllLines(flubuFile);
            if (lines.Count > 2 && !string.IsNullOrEmpty(lines[2]) && _file.Exists(lines[2]))
            {
                return lines[2];
            }

            return null;
        }

        private string GetConfigurationFileFromDefaultLocations()
        {
            foreach (var defaultSettingLocation in DefaultSettingLocations)
            {
                if (File.Exists(defaultSettingLocation))
                {
                    return defaultSettingLocation;
                }
            }

            var flubuFilePath = _buildScriptLocator.FindFlubuFile();
            if (!string.IsNullOrEmpty(flubuFilePath))
            {
                var flubuFileDir = Path.GetDirectoryName(flubuFilePath);
                foreach (var defaultSettingLocation in DefaultSettingLocations)
                {
                    var settingsLocation = Path.Combine(flubuFileDir, defaultSettingLocation);
                    if (File.Exists(settingsLocation))
                    {
                        return settingsLocation;
                    }
                }
            }

            return "flubusettings.json";
        }
    }
}
