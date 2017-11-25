using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlubuCore.Context;
using System.Threading;

namespace FlubuCore.Tasks
{
    /// <inheritdoc />
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase<TResult, TTask> : ITaskOfT<TResult, TTask> where TTask : class, ITask
    {
        private int _retriedTimes;

        private List<(Expression<Action<TTask>> TaskMethod, string ArgKey)> _fromArguments = new List<(Expression<Action<TTask>> TaskMethod, string ArgKey)>();

        /// <summary>
        ///     Gets a value indicating whether this instance is safe to execute in dry run mode.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is safe to execute in dry run mode; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSafeToExecuteInDryRun => false;

        /// <summary>
        /// Stopwatch for timings.
        /// </summary>
        public Stopwatch TaskStopwatch { get; } = new Stopwatch();

        /// <summary>
        /// Message that will be displayed when executing task.
        /// </summary>
        protected virtual string DescriptionForLog => null;

        /// <summary>
        /// Should we fail the task if an error occurs.
        /// </summary>
        protected bool DoNotFail { get; private set; }

        /// <summary>
        /// Do retry if set to true.
        /// </summary>
        protected bool DoRetry { get; private set; }

        /// <summary>
        /// Delay in ms between retries.
        /// </summary>
        protected int RetryDelay { get; private set; }

        /// <summary>
        /// Task context. It will be set after the execute method.
        /// </summary>
        protected  ITaskContext Context { get; private set; }

        /// <summary>
        /// If set to true, task should not log anything.
        /// </summary>
        protected bool DoNotLog { get; private set; }

        /// <summary>
        /// Number of retries in case of an exception.
        /// </summary>
        protected int NumberOfRetries { get; private set; }
        
        /// <summary>
        ///     Gets a value indicating whether the duration of the task should be logged after the task
        ///     has finished.
        /// </summary>
        /// <value><c>true</c> if duration should be logged; otherwise, <c>false</c>.</value>
        protected virtual bool LogDuration => false;

        /// <inheritdoc />
        public TTask DoNotFailOnError()
        {
            DoNotFail = true;

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask NoLog()
        {
            DoNotLog = true;
            return this as TTask;
        }


        public TTask FromArgument(Expression<Action<TTask>> taskMethod, string argKey, string help = null)
        {
            _fromArguments.Add((taskMethod, argKey));
            return this as TTask;
        }

        /// <summary>
        /// Log info if task logging is not disabled.
        /// </summary>
        /// <param name="message"></param>
        protected void DoLogInfo(string message)
        {
            if (DoNotLog || Context == null)
                return;

            Context.LogInfo(message);
        }

        /// <summary>
        /// Log error if task logging is not disabled.
        /// </summary>
        /// <param name="message"></param>
        protected void DoLogError(string message)
        {
            if (DoNotLog || Context == null)
                return;

            Context.LogError(message);
        }


        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="numberOfRetries">Number of retries before task fails.</param>
        /// <param name="delay">Delay time in miliseconds between retries.</param>
        /// <returns></returns>
        public TTask Retry(int numberOfRetries, int delay = 500)
        {
            DoRetry = true;
            NumberOfRetries = numberOfRetries;
            RetryDelay = delay;
            return this as TTask;
        }

        /// <inheritdoc />
        public void ExecuteVoid(ITaskContext context)
        {
            Execute(context);
        }

        /// <inheritdoc />
        public async Task ExecuteVoidAsync(ITaskContext context)
        {
            await ExecuteAsync(context);
        }

        /// <inheritdoc />
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
        public TResult Execute(ITaskContext context)
        {
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;

            Context = context ?? throw new ArgumentNullException(nameof(context));
            TaskStopwatch.Start();

            if (!string.IsNullOrEmpty(DescriptionForLog))
            {
                DoLogInfo(DescriptionForLog);
            }

            contextInternal.IncreaseDepth();

            try
            {
                InvokeFromMethods();
                return DoExecute(contextInternal);
            }
            catch (Exception)
            {
                if (!DoRetry)
                {
                    if (DoNotFail)
                    {
                        return default(TResult);
                    }

                    throw;
                }

                while (_retriedTimes < NumberOfRetries)
                {
                    _retriedTimes++;
                    contextInternal.LogInfo($"Task failed. Retriying for {_retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    Thread.Sleep(RetryDelay);
                    return Execute(context);
                }

                if (DoNotFail)
                {
                    return default(TResult);
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

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync(ITaskContext context)
        {
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            TaskStopwatch.Start();

            if (!string.IsNullOrEmpty(DescriptionForLog))
            {
                DoLogInfo(DescriptionForLog);
            }

            contextInternal.IncreaseDepth();

            try
            {
                InvokeFromMethods();
                return await DoExecuteAsync(contextInternal);
            }
            catch (Exception)
            {
                if (!DoRetry)
                {
                    throw;
                }

                while (_retriedTimes < NumberOfRetries)
                {
                    _retriedTimes++;
                    contextInternal.LogInfo($"Task failed. Retriying for {_retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    Thread.Sleep(RetryDelay);
                    return await ExecuteAsync(context);
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
        protected abstract TResult DoExecute(ITaskContextInternal context);

        /// <summary>
        /// Virtual method defining the actual work for a task.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task<TResult> DoExecuteAsync(ITaskContextInternal context)
        {
            return await Task.Run(() => DoExecute(context));
            
        }

        private void InvokeFromMethods()
        {
            if (_fromArguments.Count == 0)
            {
                return;
            }

            foreach (var fromArgument in _fromArguments)
            {
                if (!Context.ScriptArgs.ContainsKey(fromArgument.ArgKey))
                {
                    fromArgument.TaskMethod.Compile().Invoke(this as TTask);
                    return;
                }

                string value = Context.ScriptArgs[fromArgument.ArgKey];
                MethodParameterModifier parameterModifier = new MethodParameterModifier();
                var newExpression = (Expression<Action<TTask>>)parameterModifier.Modify(fromArgument.TaskMethod, new List<string>() { value });
                var action = newExpression.Compile();
                action.Invoke(this as TTask);
            }
        }
    }
}