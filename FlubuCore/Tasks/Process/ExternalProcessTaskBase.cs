using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Process
{
    public abstract class ExternalProcessTaskBase<TTask> : TaskBase<int, TTask>, IExternalProcess<TTask>
        where TTask : class, ITask
    {
        /// <summary>
        /// Gets or sets working folder.
        /// </summary>
        /// <value>
        /// Working folder.
        /// </value>
        protected string ExecuteWorkingFolder { get; set; }

        /// <summary>
        /// Executable path.
        /// </summary>
        protected string ExecutablePath { get; set; }

        /// <summary>
        /// Do not log to console if possible.
        /// </summary>
        protected bool NoOutputLog { get; set; }

        internal List<string> Arguments { get; } = new List<string>();

        protected TTask InsertArgument(int index, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                 Arguments.Insert(index, arg);
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                Arguments.Add(arg);
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(params string[] args)
        {
            Arguments.AddRange(args);
            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WorkingFolder(string folder)
        {
            ExecuteWorkingFolder = folder;
            return this as TTask;
        }

        /// <inheritdoc />
        public TTask DoNotLogOutput()
        {
            NoOutputLog = true;
            return this as TTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Clear all arguments for command line executable.
        /// </summary>
        /// <returns></returns>
        public TTask ClearArguments()
        {
            Arguments.Clear();
            return this as TTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Full file path to the executable.
        /// </summary>
        /// <param name="executableFullFilePath"></param>
        /// <returns></returns>
        public TTask Executable(string executableFullFilePath)
        {
            ExecutablePath = executableFullFilePath;
            return this as TTask;
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            PrepareExecutableParameters(context);

            IRunProgramTask task = context.Tasks().RunProgramTask(ExecutablePath);

            if (NoOutputLog)
                task.DoNotLogOutput();

            if (DoNotLog)
                task.NoLog();

            task
                .CaptureErrorOutput()
                .CaptureOutput()
                .WorkingFolder(ExecuteWorkingFolder)
                .WithArguments(Arguments.ToArray());

            return task.Execute(context);
        }

        protected virtual void PrepareExecutableParameters(ITaskContextInternal context)
        {
        }
    }
}
