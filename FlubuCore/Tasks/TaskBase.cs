using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Model;
using System.Threading;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase<T> : ITaskOfT<T>
    {
        private int retriedTimes = 0;

        /// <summary>
        ///     Gets a value indicating whether this instance is safe to execute in dry run mode.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is safe to execute in dry run mode; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSafeToExecuteInDryRun => false;

        public Stopwatch TaskStopwatch { get; } = new Stopwatch();

        protected virtual string DescriptionForLog => null;

        protected bool DoNotFail { get; private set; }

        protected bool DoRetry { get; private set; }

        protected int RetryDelay { get; private set; }

        protected int NumberOfRetries { get; private set; }
        
        /// <summary>
        ///     Gets a value indicating whether the duration of the task should be logged after the task
        ///     has finished.
        /// </summary>
        /// <value><c>true</c> if duration should be logged; otherwise, <c>false</c>.</value>
        protected virtual bool LogDuration => false;

        public ITaskOfT<T> DoNotFailOnError()
        {
            DoNotFail = true;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfRetries">Number of retries before task fails.</param>
        /// <param name="delay">Delay time in miliseconds between retries.</param>
        /// <returns></returns>
        public ITaskOfT<T> Retry(int numberOfRetries, int delay = 500)
        {
            DoRetry = true;
            NumberOfRetries = numberOfRetries;
            RetryDelay = delay;
            return this;
        }

        public void ExecuteVoid(ITaskContext context)
        {
            Execute(context);
        }

        public async Task ExecuteVoidAsync(ITaskContext context)
        {
            await ExecuteAsync(context);
        }

        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <remarks>
        ///     This method implements the basic reporting and error handling for
        ///     classes which inherit the <see>
        ///         <cref>TaskBase</cref>
        ///     </see>
        ///     class.
        /// </remarks>
        /// <param name="context">The script execution environment.</param>
        public T Execute(ITaskContext context)
        {
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;
            
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TaskStopwatch.Start();

            if (!string.IsNullOrEmpty(DescriptionForLog))
            {
                contextInternal.LogInfo(DescriptionForLog);
            }

            contextInternal.IncreaseDepth();

            try
            {
                return DoExecute(contextInternal);
            }
            catch (Exception)
            {
                if (!DoRetry)
                {
                    if (DoNotFail)
                    {
                        return default(T);
                    }

                    throw;
                }

                while (retriedTimes < NumberOfRetries)
                {
                    retriedTimes++;
                    contextInternal.LogInfo($"Task failed. Retriying for {retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    Thread.Sleep(RetryDelay);
                    Execute(context);
                }

                if (DoNotFail)
                {
                    return default(T);
                }

                throw;
            }
            finally
            {
                TaskStopwatch.Stop();
                contextInternal.DecreaseDepth();

                if (LogDuration)
                {
                    contextInternal.LogInfo($"{DescriptionForLog} finished (took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)");
                }
            }
        }

        public async Task<T> ExecuteAsync(ITaskContext context)
        {
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TaskStopwatch.Start();

            if (!string.IsNullOrEmpty(DescriptionForLog))
            {
                contextInternal.LogInfo(DescriptionForLog);
            }

            contextInternal.IncreaseDepth();

            try
            {
                return await DoExecuteAsync(contextInternal);
            }
            catch (Exception)
            {
                if (!DoRetry)
                {
                    throw;
                }

                while (retriedTimes < NumberOfRetries)
                {
                    retriedTimes++;
                    contextInternal.LogInfo($"Task failed. Retriying for {retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    Thread.Sleep(RetryDelay);
                    Execute(context);
                }

                throw;
            }
            finally
            {
                TaskStopwatch.Stop();
                contextInternal.DecreaseDepth();

                if (LogDuration)
                {
                    contextInternal.LogInfo($"{DescriptionForLog} finished (took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)");
                }
            }
        }

        /// <summary>
        ///     Abstract method defining the actual work for a task.
        /// </summary>
        /// <remarks>This method has to be implemented by the inheriting task.</remarks>
        /// <param name="context">The script execution environment.</param>
        protected abstract T DoExecute(ITaskContextInternal context);

        /// <summary>
        /// Virtual method defining the actual work for a task.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task<T> DoExecuteAsync(ITaskContextInternal context)
        {
            return await Task.Run(() => DoExecute(context));
        }
    }
}