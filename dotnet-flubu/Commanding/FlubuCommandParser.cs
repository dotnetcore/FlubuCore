using System.IO;
using Flubu.Scripting;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;

namespace Flubu.Commanding
{
    public class FlubuCommandParser : IFlubuCommandParser
    {
        private readonly CommandLineApplication commandApp;

        private CommandOption buildBasePath;

        private CommandArgument command;

        private CommandOption configurationOption;

        private CommandOption outputOption;

        private CommandArguments parsed;

        private CommandOption projectPath;

        private CommandOption scriptPath;

        public FlubuCommandParser(CommandLineApplication commandApp)
        {
            this.commandApp = commandApp;
        }

        public CommandArguments Parse(string[] args)
        {
            parsed = new CommandArguments();

            commandApp.HelpOption("-?|-h|--help");

            command = commandApp.Argument("<COMMAND> [arguments]", "The command to execute");

            configurationOption = commandApp.Option("-c|--configuration <CONFIGURATION>", "Configuration under which to run", CommandOptionType.SingleValue);
            outputOption = commandApp.Option("-o|--output <OUTPUT_DIR>", "Directory in which to find the binaries to be run", CommandOptionType.SingleValue);
            buildBasePath = commandApp.Option("-b|--build-base-path <OUTPUT_DIR>", "Directory in which to find temporary outputs", CommandOptionType.SingleValue);
            projectPath = commandApp.Option(
                "-p|--project <PROJECT>",
                "The project to execute command on, defaults to the current directory. Can be a path to a project.json or a project directory.",
                CommandOptionType.SingleValue);

            scriptPath = commandApp.Option("-s|--script <SCRIPT>", "Build script file to use.", CommandOptionType.SingleValue);

            commandApp.OnExecute(() => PrepareDefaultArguments());

            if (args == null)
            {
                args = new string[0];
            }

            parsed.Help = true;

            var res = commandApp.Execute(args);
            return parsed;
        }

        public void ShowHelp() => commandApp.ShowHelp();

        private int PrepareDefaultArguments()
        {
            parsed.Help = false;
            //// Locate the project and get the name and full path
            parsed.ProjectPath = projectPath.Value();
            if (string.IsNullOrEmpty(parsed.ProjectPath))
            {
                parsed.ProjectPath = Directory.GetCurrentDirectory();
            }

            parsed.Output = outputOption.Value();
            parsed.BuildBasePath = buildBasePath.Value();
            parsed.Config = configurationOption.Value() ?? Constants.DefaultConfiguration;
            parsed.MainCommand = command.Value;
            parsed.Script = scriptPath.Value();
            parsed.RemainingCommands = commandApp.RemainingArguments;

            return 0;
        }
    }
}