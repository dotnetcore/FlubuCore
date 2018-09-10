using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Process
{
    public abstract class ExternalProcessTaskBase<TTask> : TaskBase<int, TTask>, IExternalProcess<TTask>
        where TTask : class, ITask
    {
        // ReSharper disable once InconsistentNaming
#pragma warning disable SA1300 // Element should begin with upper-case letter
        private List<(string argKey, string argValue, bool valueRequired)> _arguments { get; } = new List<(string argKey, string argValue, bool valueRequired)>();
#pragma warning restore SA1300 // Element should begin with upper-case letter

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

        internal List<string> GetArguments()
        {
            var argumentsFlat = new List<string>();
            foreach (var arg in _arguments)
            {
                argumentsFlat.Add(arg.argKey);

                if (!string.IsNullOrEmpty(arg.argValue))
                {
                    argumentsFlat.Add(arg.argValue);
                }
            }

            return argumentsFlat;
        }

        protected TTask InsertArgument(int index, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                 _arguments.Insert(index, (arg, null, false));
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
        protected TTask WithKArgumentsValueRequired(string argKey, string argValue)
        {
            if (!string.IsNullOrEmpty(argKey))
            {
                _arguments.Add((argKey, argValue, true));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                _arguments.Add((arg, null, false));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(params string[] args)
        {
            foreach (var arg in args)
            {
                _arguments.Add((arg, null, false));
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
            _arguments.Clear();
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

            var argumentsFlat = ValidateAndGetArgumentsFlat();

            task
                .CaptureErrorOutput()
                .CaptureOutput()
                .WorkingFolder(ExecuteWorkingFolder)
                .WithArguments(argumentsFlat.ToArray());

            return task.Execute(context);
        }

        private List<string> ValidateAndGetArgumentsFlat()
        {
            var argumentsFlat = new List<string>();

            foreach (var arg in _arguments)
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

            return argumentsFlat;
        }

        protected virtual void PrepareExecutableParameters(ITaskContextInternal context)
        {
        }
    }
}
