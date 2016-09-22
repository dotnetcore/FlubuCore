using System.Collections.Generic;
using Microsoft.DotNet.ProjectModel;

namespace Flubu.Commanding
{
    public class CommandRunner
    {
        private readonly IDictionary<string, string> commands;
        private readonly ProjectContext projectContext;

        public CommandRunner(ProjectContext project)
        {
            commands = project.ProjectFile.Commands;
            projectContext = project;
        }

        ////    var commandName = args.Command;
        ////{
        ////
        ////public int Run(DotnetCommandParams args)
        ////    if (_commands.ContainsKey(args.Command))
        ////    {
        ////        commandName = _commands[args.Command];
        ////    }

        ////    var commandFactory =
        ////        new ProjectDependenciesCommandFactory(
        ////            _projectContext.TargetFramework,
        ////            args.Config,
        ////            args.Output,
        ////            args.BuildBasePath,
        ////            _projectContext.ProjectDirectory);

        ////    var commandArgs = CommandGrammar.Process(commandName, v => null, false);

        ////    Reporter.Verbose.WriteLine($"Running {commandName} {string.Join(" ", args.RemainingArguments)}");

        ////    var command = commandFactory.Create(
        ////            commandArgs.First(),
        ////            commandArgs.Skip(1).Concat(args.RemainingArguments),
        ////            _projectContext.TargetFramework,
        ////            args.Config);

        ////    var originalDir = Directory.GetCurrentDirectory();
        ////    try
        ////    {
        ////        Directory.SetCurrentDirectory(_projectContext.ProjectDirectory);
        ////        Reporter.Verbose.WriteLine("Working directory = " + _projectContext.ProjectDirectory);

        ////        return command
        ////            .ForwardStdErr()
        ////            .ForwardStdOut()
        ////            .Execute()
        ////            .ExitCode;
        ////    }
        ////    finally
        ////    {
        ////        Directory.SetCurrentDirectory(originalDir);
        ////    }
        ////}
    }
}