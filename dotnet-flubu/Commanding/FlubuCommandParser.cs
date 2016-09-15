using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace flubu.Commanding
{
    public interface IFlubuCommandParser
    {
        Command Parse(string[] args);
    }

    public class FlubuCommandParser : IFlubuCommandParser
    {
        public FlubuCommandParser(CommandLineApplication commandApp, ILogger<FlubuCommandParser> log)
        {
            _commandApp = commandApp;
            _log = log;
        }

        private CommandOption _buildBasePath;
        private CommandArgument _command;
        private CommandOption _configurationOption;
        private CommandOption _frameworkOption;
        private CommandOption _helpOption;
        private CommandOption _outputOption;
        private CommandOption _parentProcessIdOption;
        private CommandOption _projectPath;
        private readonly CommandLineApplication _commandApp;
        private readonly ILogger<FlubuCommandParser> _log;
        private Command _parsed;

        private int PrepareDefaultArguments()
        {
            _parsed = new Command();

            // Locate the project and get the name and full path
            _parsed.ProjectPath = _projectPath.Value();
            if (string.IsNullOrEmpty(_parsed.ProjectPath))
            {
                _parsed.ProjectPath = Directory.GetCurrentDirectory();
            }

            if (_parentProcessIdOption.HasValue())
            {
                int processId;

                if (!int.TryParse(_parentProcessIdOption.Value(), out processId))
                {
                    throw new InvalidOperationException(
                        $"Invalid process id '{_parentProcessIdOption.Value()}'. Process id must be an integer.");
                }

                _parsed.ParentProcessId = processId;
            }

            if (_frameworkOption.HasValue())
            {
                _parsed.Framework = NuGetFramework.Parse(_frameworkOption.Value());
            }

            _parsed.Output = _outputOption.Value();
            _parsed.BuildBasePath = _buildBasePath.Value();
            _parsed.Config = _configurationOption.Value() ?? Constants.DefaultConfiguration;
            _parsed.MainCommand = _command.Value;

            _parsed.RemainingCommands = _commandApp.RemainingArguments;

            _parsed.Help = false;
            _log.LogInformation($"c:{_parsed.MainCommand}");
            return 0;
        }

        public Command Parse(string[] args)
        {
            _helpOption = _commandApp.HelpOption("-?|-h|--help");

            _command = _commandApp.Argument("<COMMAND> [arguments]", "The command to execute");

            _configurationOption = _commandApp.Option("-c|--configuration <CONFIGURATION>","Configuration under which to run",CommandOptionType.SingleValue);
            _frameworkOption = _commandApp.Option("-f|--framework <FRAMEWORK>","Looks for command binaries for a specific framework",CommandOptionType.SingleValue);
            _outputOption = _commandApp.Option("-o|--output <OUTPUT_DIR>","Directory in which to find the binaries to be run",CommandOptionType.SingleValue);
            _buildBasePath = _commandApp.Option("-b|--build-base-path <OUTPUT_DIR>","Directory in which to find temporary outputs",CommandOptionType.SingleValue);
            _projectPath = _commandApp.Option("-p|--project <PROJECT>","The project to execute command on, defaults to the current directory. Can be a path to a project.json or a project directory.",CommandOptionType.SingleValue);
            _parentProcessIdOption = _commandApp.Option("--parentProcessId","Used by IDEs to specify their process ID. Command will exit if the parent process does.",CommandOptionType.SingleValue);

            _commandApp.OnExecute(()=>PrepareDefaultArguments());

            _commandApp.Execute(args);

            return _parsed;
        }

        public void ShowHelp() => _commandApp.ShowHelp();

        public void ShowHint() => _commandApp.ShowHint();
    }
}