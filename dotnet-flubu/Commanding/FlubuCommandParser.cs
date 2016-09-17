using flubu.Scripting;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Frameworks;
using System;
using System.IO;

namespace flubu.Commanding
{
    public interface IFlubuCommandParser
    {
        CommandArguments Parse(string[] args);

        void ShowHelp();
    }

    public class FlubuCommandParser : IFlubuCommandParser
    {
        private readonly CommandLineApplication _commandApp;

        private CommandOption _buildBasePath;

        private CommandArgument _command;

        private CommandOption _configurationOption;

        private CommandOption _outputOption;

        private CommandArguments _parsed;

        private CommandOption _projectPath;
        private CommandOption _scriptPath;

        public FlubuCommandParser(CommandLineApplication commandApp)
        {
            _commandApp = commandApp;
        }
        public CommandArguments Parse(string[] args)
        {
            _parsed = new CommandArguments();

            _commandApp.HelpOption("-?|-h|--help");

            _command = _commandApp.Argument("<COMMAND> [arguments]", "The command to execute");

            _configurationOption = _commandApp.Option("-c|--configuration <CONFIGURATION>", "Configuration under which to run", CommandOptionType.SingleValue);
            _outputOption = _commandApp.Option("-o|--output <OUTPUT_DIR>", "Directory in which to find the binaries to be run", CommandOptionType.SingleValue);
            _buildBasePath = _commandApp.Option("-b|--build-base-path <OUTPUT_DIR>", "Directory in which to find temporary outputs", CommandOptionType.SingleValue);
            _projectPath = _commandApp.Option("-p|--project <PROJECT>", "The project to execute command on, defaults to the current directory. Can be a path to a project.json or a project directory.", CommandOptionType.SingleValue);
            _scriptPath = _commandApp.Option("-s|--script <SCRIPT>", "Build script file to use.", CommandOptionType.SingleValue);

            _commandApp.OnExecute(() => PrepareDefaultArguments());

            if (args == null)
                args = new string[0];

            _parsed.Help = true;

            int res = _commandApp.Execute(args);
            return _parsed;
        }

        public void ShowHelp() => _commandApp.ShowHelp();

        private int PrepareDefaultArguments()
        {
            _parsed.Help = false;
            // Locate the project and get the name and full path
            _parsed.ProjectPath = _projectPath.Value();
            if (string.IsNullOrEmpty(_parsed.ProjectPath))
            {
                _parsed.ProjectPath = Directory.GetCurrentDirectory();
            }

            _parsed.Output = _outputOption.Value();
            _parsed.BuildBasePath = _buildBasePath.Value();
            _parsed.Config = _configurationOption.Value() ?? Constants.DefaultConfiguration;
            _parsed.MainCommand = _command.Value;
            _parsed.Script = _scriptPath.Value();
            _parsed.RemainingCommands = _commandApp.RemainingArguments;

            return 0;
        }
    }
}