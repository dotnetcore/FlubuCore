using System.IO;
using FlubuCore.Scripting;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Cli.Flubu.Commanding
{
    public class FlubuCommandParser : IFlubuCommandParser
    {
        private readonly CommandLineApplication _commandApp;

        private CommandArgument _command;

        private CommandOption _configurationOption;

        private CommandOption _outputOption;

        private CommandArguments _parsed;

        private CommandOption _scriptPath;

        private CommandOption _scriptAssembly;

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

            _scriptPath = _commandApp.Option("-s|--script <SCRIPT>", "Build script file to use.", CommandOptionType.SingleValue);
            _scriptAssembly = _commandApp.Option("-a|--assembly <SCRIPT>", "Load build script from assembly.", CommandOptionType.SingleValue);

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
            _parsed.MainCommand = _command.Value;
            _parsed.Script = _scriptPath.Value();
            _parsed.ScriptAssembly = _scriptAssembly.Value();
            _parsed.RemainingCommands = _commandApp.RemainingArguments;

            return 0;
        }
    }
}