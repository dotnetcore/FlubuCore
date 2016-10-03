using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public class ExecuteDotnetTask : TaskBase
    {
        private readonly string _command;
        private readonly List<string> _arguments = new List<string>();
        private string _workingFolder;
        private string _dotnetExecutable;

        public ExecuteDotnetTask(string command)
        {
            _command = command;
        }

        public ExecuteDotnetTask(StandardDotnetCommands command)
        {
            _command = command.ToString().ToLowerInvariant();
        }

        public ExecuteDotnetTask WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this;
        }

        public ExecuteDotnetTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        public ExecuteDotnetTask WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return this;
        }

        public ExecuteDotnetTask DotnetExecutable(string fullPath)
        {
            _dotnetExecutable = fullPath;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            string program = _dotnetExecutable;

            if (string.IsNullOrEmpty(program))
            {
                program = context.GetDotnetExecutable();
            }

            if (string.IsNullOrEmpty(program))
            {
                context.Fail("Dotnet executable not set!", -1);
                return -1;
            }

            RunProgramTask task = new RunProgramTask(program);

            return task
                .WithArguments(_command)
                .WithArguments(_arguments.ToArray())
                .WorkingFolder(_workingFolder)
                .Execute(context);
        }
    }
}