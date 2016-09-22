using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.Cli.Utils;

namespace Flubu.Tasks.Process
{
    public class RunProgramTask : TaskBase
    {
        private string _programToExecute;

        public RunProgramTask(string programToExecute)
        {
            _programToExecute = programToExecute;
        }

        public override string Description => "Run program";

        protected override void DoExecute(ITaskContext context)
        {
            // context.WriteMessage(
            // $"Running program '{programExePath}': (work.dir='{workingDirectory}',args = '{null}', timeout = {executionTimeout})");
            CommandFactory commandFactory = new CommandFactory();

            ICommand command = commandFactory.Create(_programToExecute, new List<string>());

            string originalDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(".");

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
