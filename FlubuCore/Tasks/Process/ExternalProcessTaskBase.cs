using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Process
{
    public abstract class ExternalProcessTaskBase<T> : TaskBase<int>, IExternalProcess<T> where T : class, ITask
    {
        /// <summary>
        /// Arguments for the command line.
        /// </summary>
        private readonly List<string> _arguments = new List<string>();

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

        public List<string> GetArguments()
        {
            return _arguments;
        }

        protected T InsertArgument(int index, string arg)
        {
            _arguments.Insert(index, arg);
            return this as T;
        }
        /// <inheritdoc />
        public T WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this as T;
        }

        /// <inheritdoc />
        public T WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
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

        /// <inheritdoc />
        /// <summary>
        /// Clear all arguments for command line executable.
        /// </summary>
        /// <returns></returns>
        public T ClearArguments()
        {
            _arguments.Clear();
            return this as T;
        }

        /// <inheritdoc />
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
                .WithArguments(_arguments.ToArray());

            return task.Execute(context);
        }

        protected virtual void PrepareExecutableParameters(ITaskContextInternal context)
        {
        }
    }
}
