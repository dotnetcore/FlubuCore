using flubu.Tasks;
using System;
using System.Diagnostics;

namespace flubu
{
    /// <summary>
    /// A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        /// <summary>
        /// Gets the task description.
        /// </summary>
        /// <value>The task description.</value>
        public abstract string Description { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is safe to execute in dry run mode.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is safe to execute in dry run mode; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSafeToExecuteInDryRun
        {
            get { return false; }
        }

        public Stopwatch TaskStopwatch
        {
            get { return taskStopwatch; }
        }

        /// <summary>
        /// Executes the task using the specified script execution environment.
        /// </summary>
        /// <remarks>This method implements the basic reporting and error handling for
        /// classes which inherit the <see cref="TaskBase"/> class.</remarks>
        /// <param name="context">The script execution environment.</param>
        public void Execute (ITaskContext context)
        {
            if (context == null)
                throw new ArgumentNullException (nameof(context));

            taskStopwatch.Start();

            context.WriteInfo("{0}", DescriptionForLog);
            context.IncreaseDepth();

            try
            {
                DoExecute (context);
                taskStopwatch.Stop();
            }
            finally
            {
                context.DecreaseDepth();

                if (LogDuration)
                {
                    context.WriteInfo(
                        "{0} finished (took {1} seconds)", 
                        DescriptionForLog, 
                        (int)taskStopwatch.Elapsed.TotalSeconds);
                }
            }
        }

        protected virtual string DescriptionForLog
        {
            get { return Description; }
        }

        /// <summary>
        /// Gets a value indicating whether the duration of the task should be logged after the task
        /// has finished.
        /// </summary>
        /// <value><c>true</c> if duration should be logged; otherwise, <c>false</c>.</value>
        protected virtual bool LogDuration
        {
            get { return false; }
        }

        /// <summary>
        /// Abstract method defining the actual work for a task.
        /// </summary>
        /// <remarks>This method has to be implemented by the inheriting task.</remarks>
        /// <param name="context">The script execution environment.</param>
        protected abstract void DoExecute (ITaskContext context);

        private readonly Stopwatch taskStopwatch = new Stopwatch();
    }
}
