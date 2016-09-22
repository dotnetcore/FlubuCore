using System;
using System.Diagnostics;

namespace Flubu.Tasks
{
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is safe to execute in dry run mode.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is safe to execute in dry run mode; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSafeToExecuteInDryRun
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets the task description.
        /// </summary>
        /// <value>The task description.</value>
        public abstract string Description { get; }

        public Stopwatch TaskStopwatch { get; } = new Stopwatch();

        protected virtual string DescriptionForLog
        {
            get { return Description; }
        }

        /// <summary>
        ///     Gets a value indicating whether the duration of the task should be logged after the task
        ///     has finished.
        /// </summary>
        /// <value><c>true</c> if duration should be logged; otherwise, <c>false</c>.</value>
        protected virtual bool LogDuration
        {
            get { return false; }
        }

        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <remarks>
        ///     This method implements the basic reporting and error handling for
        ///     classes which inherit the <see cref="TaskBase" /> class.
        /// </remarks>
        /// <param name="context">The script execution environment.</param>
        public int Execute(ITaskContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TaskStopwatch.Start();

            context.WriteMessage(DescriptionForLog);
            context.IncreaseDepth();

            try
            {
                DoExecute(context);
                return 0;
            }
            finally
            {
                TaskStopwatch.Stop();
                context.DecreaseDepth();

                if (LogDuration)
                {
                    context.WriteMessage($"{DescriptionForLog} finished (took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)");
                }
            }
        }

        /// <summary>
        ///     Abstract method defining the actual work for a task.
        /// </summary>
        /// <remarks>This method has to be implemented by the inheriting task.</remarks>
        /// <param name="context">The script execution environment.</param>
        protected abstract void DoExecute(ITaskContext context);
    }
}