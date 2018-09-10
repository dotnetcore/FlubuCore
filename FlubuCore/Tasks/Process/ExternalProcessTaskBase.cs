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

        internal List<(string argKey, string argValue, bool valueRequired)> Arguments { get; } = new List<(string argKey, string argValue, bool valueRequired)>();

        protected TTask InsertArgument(int index, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                 Arguments.Insert(index, (arg, null, false));
            }

            return this as TTask;
        }

        /// <summary>
        /// Adds argument with corresponding value. eg --Framework .net462 where --Framework is key and .net462 is value.
        /// If value is null or empty task fails on execution. if key is null both argument's are ignored.
        /// </summary>
        /// <param name="argKey"></param>
        /// <param name="argValue"></param>
        /// <returns></returns>
        protected TTask WithArgumentsValueRequired(string argKey, string argValue)
        {
            if (!string.IsNullOrEmpty(argKey))
            {
                Arguments.Add((argKey, argValue, true));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                Arguments.Add((arg, null, false));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(params string[] args)
        {
            foreach (var arg in args)
            {
                Arguments.Add((arg, null, false));
            }

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

            var argumentsFlat = new List<string>();

            foreach (var arg in Arguments)
            {
                argumentsFlat.Add(arg.argKey);

                if (string.IsNullOrEmpty(arg.argValue))
                {
                    if (arg.valueRequired)
                    {
                        throw new TaskExecutionException($"Argument key {arg.argKey} requires value.", 0);
                    }
                }
                else
                {
                    argumentsFlat.Add(arg.argValue);
                }
            }

            task
                .CaptureErrorOutput()
                .CaptureOutput()
                .WorkingFolder(ExecuteWorkingFolder)
                .WithArguments(argumentsFlat.ToArray());

            return task.Execute(context);
        }

        protected virtual void PrepareExecutableParameters(ITaskContextInternal context)
        {
        }
    }
}
