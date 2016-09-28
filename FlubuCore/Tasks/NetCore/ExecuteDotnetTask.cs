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

        public ExecuteDotnetTask(string command)
        {
            _command = command;
        }

        public ExecuteDotnetTask(StandardDotnetCommands command)
        {
            _command = command.ToString().ToLowerInvariant();
        }

        public override string Description => $"Execute dotnet command";

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

        protected override int DoExecute(ITaskContext context)
        {
            RunProgramTask task = new RunProgramTask("dotnet");

            return task
                .WithArguments(_command)
                .WithArguments(_arguments.ToArray())
                .WorkingFolder(_workingFolder)
                .Execute(context);
        }
    }
}