using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace FlubuCore.Tasks.NetCore
{
    public abstract class ExecuteDotnetTaskBase<TTask> : ExternalProcessTaskBase<int, TTask>
        where TTask : class, ITask
    {
        public ExecuteDotnetTaskBase(string command)
        {
            AddAdditionalOptionPrefix(command);
            Command = command;
        }

        public ExecuteDotnetTaskBase(StandardDotnetCommands command)
        {
            AddAdditionalOptionPrefix(command.ToString());
            Command = command.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Dotnet command to be executed.
        /// </summary>
        public string Command { get; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(ExecutablePath))
            {
                ExecutablePath = context.Properties.GetDotnetExecutable();
                if (string.IsNullOrEmpty(ExecutablePath))
                {
                    context.Fail("Dotnet executable not set!", -1);
                    return -1;
                }
            }

            KeepProgramErrorOutput = true;
            KeepProgramOutput = true;
            return base.DoExecute(context);
        }

        protected override void BeforeExecute(ITaskContextInternal context, IRunProgramTask runProgramTask)
        {
            runProgramTask.WithArguments(Command);
            base.BeforeExecute(context, runProgramTask);
        }
    }
}