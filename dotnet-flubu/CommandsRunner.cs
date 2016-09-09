using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.Cli.Utils.CommandParsing;
using Microsoft.DotNet.ProjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace flubu
{
    public class CommandRunner
    {
        private readonly IDictionary<string, string> _commands;
        private readonly ProjectContext _projectContext;

        public CommandRunner(ProjectContext project)
        {
            _commands = project.ProjectFile.Commands;
            _projectContext = project;
        }

        public int Run(DotnetCommandParams args)
        {
            var commandName = args.Command;
            if (_commands.ContainsKey(args.Command))
            {
                commandName = _commands[args.Command];
            }

            var commandFactory =
                new ProjectDependenciesCommandFactory(
                    _projectContext.TargetFramework,
                    args.Config,
                    args.Output,
                    args.BuildBasePath,
                    _projectContext.ProjectDirectory);

            var commandArgs = CommandGrammar.Process(commandName, v => null, false);

            Reporter.Verbose.WriteLine($"Running {commandName} {string.Join(" ", args.RemainingArguments)}");

            var command = commandFactory.Create(
                    commandArgs.First(),
                    commandArgs.Skip(1).Concat(args.RemainingArguments),
                    _projectContext.TargetFramework,
                    args.Config);

            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_projectContext.ProjectDirectory);
                Reporter.Verbose.WriteLine("Working directory = " + _projectContext.ProjectDirectory);

                return command
                    .ForwardStdErr()
                    .ForwardStdOut()
                    .Execute()
                    .ExitCode;
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
    }
}