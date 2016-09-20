using System.IO;

namespace flubu.core.Tasks.dotnet
{
    public class ExecuteDotnetTask : TaskBase
    {
        public override string Description => $"Execute dotnet command";
        public string Command { get; set; }

        public ExecuteDotnetTask(string command)
        {
            Command = command;
        }

        protected override void DoExecute(ITaskContext context)
        {
            //ProjectDependenciesCommandFactory commandFactory =
            //    new ProjectDependenciesCommandFactory(
            //        NuGetFramework.AnyFramework,
            //        "Debug",
            //        ".",
            //        ".",
            //        ".");

            //string[] commandArgs = CommandGrammar.Process(Command, v => null, false);

            //Reporter.Verbose.WriteLine($"Running {Command}");

            //ICommand command = commandFactory.Create(
            //        Command,
            //        null,
            //        NuGetFramework.AnyFramework,
            //        "Debug");

            //var originalDir = Directory.GetCurrentDirectory();
            //try
            //{
            //    Directory.SetCurrentDirectory(".");
            //    Reporter.Verbose.WriteLine("Working directory = " + ".");

            //    command
            //        .ForwardStdErr()
            //        .ForwardStdOut()
            //        .Execute();
            //        //.ExitCode;
            //}
            //finally
            //{
            //    Directory.SetCurrentDirectory(originalDir);
            //}
        }
    }
}
