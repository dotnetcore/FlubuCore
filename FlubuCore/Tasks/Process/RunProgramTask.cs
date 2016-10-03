using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Process
{
    public class RunProgramTask : TaskBase
    {
        private readonly string _programToExecute;
        private readonly List<string> _arguments = new List<string>();

        private ICommandFactory _commandFactory;
        private string _workingFolder;

        public RunProgramTask(string programToExecute)
        {
            _programToExecute = programToExecute;
        }

        public RunProgramTask(ICommandFactory commandFactory, string programToExecute)
        {
            _commandFactory = commandFactory;
            _programToExecute = programToExecute;
        }

        public RunProgramTask WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this;
        }

        public RunProgramTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        public RunProgramTask WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            context.WriteMessage(
                $"Running program '{_programToExecute}': (work.dir='{_workingFolder}',args = '{_arguments.ListToArgsString()}')");

            if (_commandFactory == null)
            {
                _commandFactory = new CommandFactory();
            }

            string currentDirectory = Directory.GetCurrentDirectory();

            FileInfo info = new FileInfo(_programToExecute);

            ICommand command = _commandFactory.Create(info.FullName, _arguments);

            int res = command
                .CaptureStdErr()
                .CaptureStdOut()
                .WorkingDirectory(_workingFolder ?? currentDirectory)
                .OnErrorLine(context.WriteMessage)
                .OnOutputLine(context.WriteMessage)
                .Execute()
                .ExitCode;

            if (res != 0)
            {
                context.Fail($"External program {_programToExecute} failed with {res}. Full path:{info.FullName}", res);
            }

            return res;
        }
    }
}
