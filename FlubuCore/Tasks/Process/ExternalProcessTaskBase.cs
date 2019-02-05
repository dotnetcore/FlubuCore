using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Process
{
    public abstract class ExternalProcessTaskBase<TResult, TTask> : TaskBase<TResult, TTask>, IExternalProcess<TTask>
        where TTask : class, ITask
    {
        // ReSharper disable once InconsistentNaming
#pragma warning disable SA1300 // Element should begin with upper-case letter
        private List<(string argKey, string argValue, bool valueRequired, bool maskArg)> _arguments { get; } = new List<(string argKey, string argValue, bool valueRequired, bool maskArg)>();
#pragma warning restore SA1300 // Element should begin with upper-case letter

        /// <summary>
        /// Gets or sets working folder.
        /// </summary>
        /// <value>
        /// Working folder.
        /// </value>
        protected string ExecuteWorkingFolder { get; set; }

        protected string ProgramOutput { get; set; }

        protected virtual bool GetProgramOutput { get; } = false;

        /// <summary>
        /// Executable path.
        /// </summary>
        protected string ExecutablePath { get; set; }

        /// <summary>
        /// Do not log to console if possible.
        /// </summary>
        protected bool NoOutputLog { get; set; }

        protected internal List<string> GetArguments()
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

        protected TTask InsertArgument(int index, string arg, bool maskArg = false)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                 _arguments.Insert(index, (arg, null, false, maskArg));
            }

            return this as TTask;
        }

        /// <summary>
        /// Adds argument with corresponding value. eg --Framework .net462 where --Framework is key and .net462 is value.
        /// If value is null or empty task fails on execution. if key is null both argument's are ignored.
        /// </summary>
        /// <param name="argKey"></param>
        /// <param name="argValue"></param>
        /// <param name="maskValue">If <c>true</c> value is masked.</param>
        /// <returns></returns>
        protected TTask WithArgumentsValueRequired(string argKey, string argValue, bool maskValue = false)
        {
            if (!string.IsNullOrEmpty(argKey))
            {
                _arguments.Add((argKey, argValue, true, maskValue));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(string arg, bool maskArg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                _arguments.Add((arg, null, false, maskArg));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(params string[] args)
        {
            foreach (var arg in args)
            {
                _arguments.Add((arg, null, false, false));
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
        protected override TResult DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(ExecutablePath))
            {
                throw new TaskExecutionException($"{nameof(ExecutablePath)} must be set.", 5);
            }

            IRunProgramTask task = context.Tasks().RunProgramTask(ExecutablePath);

            if (NoOutputLog)
                task.DoNotLogOutput();

            if (DoNotLog)
                task.NoLog();

            BeforeExecute(context);

            var argumentsFlat = ValidateAndGetArgumentsFlat();

            task
                .CaptureErrorOutput()
                .CaptureOutput()
                .WorkingFolder(ExecuteWorkingFolder);
            foreach (var arg in argumentsFlat)
            {
                task.WithArguments(arg.arg, arg.maskArg);
            }

            var result = task.Execute(context);

            if (GetProgramOutput)
            {
                ProgramOutput = task.GetOutput();
            }

            if (typeof(TResult) == typeof(int))
            {
                return (TResult)(object)result;
            }

            return default(TResult);
        }

        protected List<(string arg, bool maskArg)> ValidateAndGetArgumentsFlat()
        {
            var argumentsFlat = new List<(string arg, bool maskArg)>();

            foreach (var arg in _arguments)
            {
                if (string.IsNullOrEmpty(arg.argValue))
                {
                    if (arg.valueRequired)
                    {
                        throw new TaskExecutionException($"Argument key {arg.argKey} requires value.", 0);
                    }

                    argumentsFlat.Add((arg.argKey, arg.maskArg));
                }
                else
                {
                    argumentsFlat.Add((arg.argKey, false));
                    argumentsFlat.Add((arg.argValue, arg.maskArg));
                }
            }

            return argumentsFlat;
        }

        protected virtual void BeforeExecute(ITaskContextInternal context)
        {
        }
    }
}
