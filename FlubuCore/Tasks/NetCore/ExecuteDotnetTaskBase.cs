using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public abstract class ExecuteDotnetTaskBase<TTask> : ExternalProcessTaskBase<TTask>
        where TTask : class, ITask
    {
        private string _workingFolder;
        private string _dotnetExecutable;
        private bool _doNotLogOutput;

        public ExecuteDotnetTaskBase(string command)
        {
            Command = command;
        }

        public ExecuteDotnetTaskBase(StandardDotnetCommands command)
        {
            Command = command.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Dotnet command to be executed.
        /// </summary>
        public string Command { get; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            string program = _dotnetExecutable;

            if (string.IsNullOrEmpty(program))
            {
                program = context.Properties.GetDotnetExecutable();
            }

            if (string.IsNullOrEmpty(program))
            {
                context.Fail("Dotnet executable not set!", -1);
                return -1;
            }

            BeforeExecute(context);
            IRunProgramTask task = context.Tasks().RunProgramTask(program);

            if (_doNotLogOutput)
                task.DoNotLogOutput();

            if (DoNotLog)
                task.NoLog();

            task
                .WithArguments(Command)
                .WithArguments(GetArguments().ToArray())
                .WorkingFolder(_workingFolder)
                .CaptureErrorOutput()
                .CaptureOutput()
                .ExecuteVoid(context);

            return 0;
        }

        protected virtual void BeforeExecute(ITaskContextInternal context)
        {
        }
    }
}