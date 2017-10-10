using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlubuCore.Tasks
{
    public abstract class ExternalProcessTaskBase<T, TaskResult> : TaskBase<TaskResult>, IExternalProcess<T>
        where T : ITask
    {
        protected readonly List<string> Arguments = new List<string>();

        protected string _workingFolder;

        private bool _doNotLogOutput;

        /// <inheritdoc />
        public T WithArguments(string arg)
        {
            Arguments.Add(arg);
            return (T) (object) this;
        }

        /// <inheritdoc />
        public T WithArguments(params string[] args)
        {
            Arguments.AddRange(args);
            return (T) (object) this;
        }

        /// <inheritdoc />
        public T WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return (T) (object) this;
        }

        /// <inheritdoc />
        public T DoNotLogOutput()
        {
            _doNotLogOutput = true;
            return (T) (object) this;
        }

        protected IRunProgramTask DoExecuteExternalProcessBase(ITaskContextInternal context, string programToExecute)
        {
            IRunProgramTask task = context.Tasks().RunProgramTask("sc");

            if (_doNotLogOutput)
                task.DoNotLogOutput();

            if (DoNotLog)
                task.NoLog();

            task
                .WithArguments(Arguments.ToArray())
                .CaptureErrorOutput()
                .CaptureOutput()
                .WorkingFolder(_workingFolder);

            return task;
        }
    }
}
