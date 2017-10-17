using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks
{
    public abstract class ExternalProcessTaskBase<T> : TaskBase<int>, IExternalProcess<T> where T : class, ITask
    {
        /// <summary>
        /// Arguments for the command line.
        /// </summary>
        protected readonly List<string> Arguments = new List<string>();

        /// <summary>
        /// Working folder.
        /// </summary>
        protected string ExecuteWorkingFolder;
        
        /// <summary>
        /// Executable path.
        /// </summary>
        protected string ExecutablePath;

        /// <summary>
        /// Do not log to console if possible.
        /// </summary>
        protected bool NoOutputLog;

        /// <inheritdoc />
        public T WithArguments(string arg)
        {
            Arguments.Add(arg);
            return this as T;
        }

        /// <inheritdoc />
        public T WithArguments(params string[] args)
        {
            Arguments.AddRange(args);
            return this as T;
        }

        /// <inheritdoc />
        public T WorkingFolder(string folder)
        {
            ExecuteWorkingFolder = folder;
            return this as T;
        }

        /// <inheritdoc />
        public T DoNotLogOutput()
        {
            NoOutputLog = true;
            return this as T;
        }

        /// <summary>
        /// Clear all arguments for command line executable.
        /// </summary>
        /// <returns></returns>
        public T ClearArguments()
        {
            Arguments.Clear();
            return this as T;
        }

        /// <summary>
        /// Full file path to the executable.
        /// </summary>
        /// <param name="executableFullFilePath"></param>
        /// <returns></returns>
        public T Executable(string executableFullFilePath)
        {
            ExecutablePath = executableFullFilePath;
            return this as T;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            PrepareExecutableParameters(context);

            IRunProgramTask task = context.Tasks().RunProgramTask(ExecutablePath);

            if (NoOutputLog)
                task.DoNotLogOutput();

            if (DoNotLog)
                task.NoLog();

             task
                .WithArguments(Arguments.ToArray());

            task.CaptureErrorOutput();
            task.CaptureOutput();
            task.WorkingFolder(ExecuteWorkingFolder);

            return task.Execute(context);
        }

        protected virtual void PrepareExecutableParameters(ITaskContextInternal context)
        {
        }
    }
}
