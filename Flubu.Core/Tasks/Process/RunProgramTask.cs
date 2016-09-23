using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DotNet.Cli.Utils;

namespace Flubu.Tasks.Process
{
    public class RunProgramTask : TaskBase
    {
        private readonly string _programToExecute;

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

            ICommand command = commandFactory.Create(_programToExecute, new List<string> { "--version" });

            string originalDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(".");
                using (MemoryStream s = new MemoryStream())
                using (TextWriter w = new StreamWriter(s))
                {
                    int res = command
                        .CaptureStdErr()
                        .CaptureStdOut()
                        .OnErrorLine(context.WriteMessage)
                        .OnOutputLine(context.WriteMessage)
                        .Execute()
                        .ExitCode;

                    w.Flush();

                    context.WriteMessage(Encoding.UTF8.GetString(s.ToArray()));

                    if (res != 0)
                    {
                        context.Fail($"External program failed with {res}");
                    }
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
    }
}
