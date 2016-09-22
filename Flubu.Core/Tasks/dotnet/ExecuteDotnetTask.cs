using System.IO;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.Cli.Utils.CommandParsing;
using NuGet.Frameworks;

namespace Flubu.Tasks.Dotnet
{
    public class ExecuteDotnetTask : TaskBase
    {
        public ExecuteDotnetTask(string command)
        {
            Command = command;
        }

        public override string Description => $"Execute dotnet command";

        public string Command { get; set; }

        protected override void DoExecute(ITaskContext context)
        {
            ProjectDependenciesCommandFactory commandFactory =
                new ProjectDependenciesCommandFactory(
                    NuGetFramework.AnyFramework,
                    "Debug",
                    ".",
                    ".",
                    ".");

            string[] commandArgs = CommandGrammar.Process(Command, v => null, false);

            Reporter.Verbose.WriteLine($"Running {Command}");

            ICommand command = commandFactory.Create(
                    Command,
                    null,
                    NuGetFramework.AnyFramework,
                    "Debug");

            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(".");
                Reporter.Verbose.WriteLine("Working directory = " + ".");

                command
                    .ForwardStdErr()
                    .ForwardStdOut()
                    .Execute();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
    }
}