using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public abstract class ExecuteDotnetTaskBase<TTask> : ExternalProcessTaskBase<int, TTask>
        where TTask : class, ITask
    {
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
            string program = ExecutablePath;

            if (string.IsNullOrEmpty(program))
            {
                program = context.Properties.GetDotnetExecutable();
            }

            if (string.IsNullOrEmpty(program))
            {
                context.Fail("Dotnet executable not set!", -1);
                return -1;
            }

            IRunProgramTask task = context.Tasks().RunProgramTask(program);

            if (NoOutputLog)
                task.DoNotLogOutput();

            if (DoNotLog)
                task.NoLog();

            BeforeExecute(context);
            var argumentsFlat = ValidateAndGetArgumentsFlat();

            task
                .WithArguments(Command);

            foreach (var arg in argumentsFlat)
            {
                task.WithArguments(arg.arg, arg.maskArg);
            }

            task
                .WorkingFolder(ExecuteWorkingFolder)
                .CaptureErrorOutput()
                .CaptureOutput()
                .ExecuteVoid(context);

            return 0;
        }
    }
}