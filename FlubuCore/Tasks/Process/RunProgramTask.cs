using System;
using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Process
{
    public class RunProgramTask : TaskBase<int>, IRunProgramTask
    {
        private readonly string _programToExecute;
        private readonly List<string> _arguments = new List<string>();

        private ICommandFactory _commandFactory;
        private string _workingFolder;

        public RunProgramTask(ICommandFactory commandFactory, string programToExecute)
        {
            _commandFactory = commandFactory;
            _programToExecute = programToExecute;
        }

        public IRunProgramTask WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this;
        }

        public IRunProgramTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        public IRunProgramTask WorkingFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || folder.Equals(".", StringComparison.OrdinalIgnoreCase))
                return this;

            _workingFolder = folder;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            if (_commandFactory == null)
                _commandFactory = new CommandFactory();

            string currentDirectory = Directory.GetCurrentDirectory();

            ICommand command = _commandFactory.Create(_programToExecute, _arguments);

            command
                .CaptureStdErr()
                .CaptureStdOut()
                .WorkingDirectory(_workingFolder ?? currentDirectory)
                .OnErrorLine(context.LogInfo)
                .OnOutputLine(context.LogInfo);

            context.LogInfo(
                $"Running program '{command.CommandName}':(work.dir='{_workingFolder}',args='{command.CommandArgs}')");

            int res = command.Execute()
                .ExitCode;

            if (res != 0)
                context.Fail($"External program {_programToExecute} failed with {res}.", res);

            return res;
        }
    }
}
