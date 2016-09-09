using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace flubu
{
    public class DotnetCommandParams
    {
        private readonly CommandLineApplication _app;

        private CommandArgument _command;
        private CommandOption _helpOption;
        private CommandOption _outputOption;
        private CommandOption _buildBasePath;
        private CommandOption _frameworkOption;
        private CommandOption _configurationOption;
        private CommandOption _parentProcessIdOption;
        private CommandOption _projectPath;

        public string Command { get; set; }

        public int? ParentProcessId { get; set; }

        public string Config { get; set; }

        public string BuildBasePath { get; set; }

        public string Output { get; set; }

        public string ProjectPath { get; set; }

        public NuGetFramework Framework { get; set; }

        public List<string> RemainingArguments { get; set; }

        public bool Help { get; set; }

        public DotnetCommandParams()
        {
            _app = new CommandLineApplication(false)
            {
                Name = "dotnet command",
                FullName = ".NET Project Commands",
                Description = "Project command executor for the .NET Platform"
            };

            AddDotnetCommandParams();

            Help = true;
        }

        public void ShowHint() => _app.ShowHint();

        public void ShowHelp() => _app.ShowHelp();

        public void Parse(string[] args)
        {
            _app.OnExecute(() =>
            {
                // Locate the project and get the name and full path
                ProjectPath = _projectPath.Value();
                if (string.IsNullOrEmpty(ProjectPath))
                {
                    ProjectPath = Directory.GetCurrentDirectory();
                }

                if (_parentProcessIdOption.HasValue())
                {
                    int processId;

                    if (!int.TryParse(_parentProcessIdOption.Value(), out processId))
                    {
                        throw new InvalidOperationException(
                            $"Invalid process id '{_parentProcessIdOption.Value()}'. Process id must be an integer.");
                    }

                    ParentProcessId = processId;
                }

                if (_frameworkOption.HasValue())
                {
                    Framework = NuGetFramework.Parse(_frameworkOption.Value());
                }

                Output = _outputOption.Value();
                BuildBasePath = _buildBasePath.Value();
                Config = _configurationOption.Value() ?? Constants.DefaultConfiguration;
                Command = _command.Value;

                RemainingArguments = _app.RemainingArguments;

                Help = false;

                return 0;
            });

            _app.Execute(args);
        }

        private void AddDotnetCommandParams()
        {
            _helpOption = _app.HelpOption("-?|-h|--help");

            _command = _app.Argument(
                "<COMMAND> [arguments]",
                "The command to execute");

            _configurationOption = _app.Option(
                "-c|--configuration <CONFIGURATION>",
                "Configuration under which to run",
                CommandOptionType.SingleValue);
            _frameworkOption = _app.Option(
                "-f|--framework <FRAMEWORK>",
                "Looks for command binaries for a specific framework",
                CommandOptionType.SingleValue);
            _outputOption = _app.Option(
                "-o|--output <OUTPUT_DIR>",
                "Directory in which to find the binaries to be run",
                CommandOptionType.SingleValue);
            _buildBasePath = _app.Option(
                "-b|--build-base-path <OUTPUT_DIR>",
                "Directory in which to find temporary outputs",
                CommandOptionType.SingleValue);
            // _noBuildOption =
            //     _app.Option("--no-build", "Do not build project before testing", CommandOptionType.NoValue);
            _projectPath = _app.Option(
                "-p|--project <PROJECT>",
                "The project to execute command on, defaults to the current directory. Can be a path to a project.json or a project directory.",
                CommandOptionType.SingleValue);
            _parentProcessIdOption = _app.Option(
                "--parentProcessId",
                "Used by IDEs to specify their process ID. Command will exit if the parent process does.",
                CommandOptionType.SingleValue);
        }
    }
}