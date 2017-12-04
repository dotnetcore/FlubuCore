using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FlubuCore.Context;
using System.Threading;
using FlubuCore.Tasks.Attributes;
using Microsoft.Build.Utilities;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Tasks
{
    /// <inheritdoc />
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase<TResult, TTask> : TaskHelp, ITaskOfT<TResult, TTask> where TTask : class, ITask
    {
        private int _retriedTimes;

        private string _taskName;

        private List<(Expression<Action<TTask>> TaskMethod, string ArgKey, bool includeParameterlessMethodByDefault)> _fromArguments = new List<(Expression<Action<TTask>> TaskMethod, string ArgKey, bool includeParameterlessMethodByDefault)>();
        
        internal Dictionary<string, string> ArgumentHelp { get;  } = new Dictionary<string, string>();

        /// <summary>
        /// Stopwatch for timings.
        /// </summary>
        internal Stopwatch TaskStopwatch { get; } = new Stopwatch();

        protected abstract string Description { get; set; }

        protected string TaskName
        {
            get
            {
                if (!string.IsNullOrEmpty(_taskName))
                {
                    return _taskName;
                }

                var type = typeof(TTask);
                return type.Name;
            }

            set => _taskName = value;
        }

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

        [DisableForMember]
        public TTask ForMember(Expression<Action<TTask>> taskMethod, string argKey, string help = null, bool includeParameterlessMethodByDefault = true)
        {
            string key = argKey.TrimStart('-');
            _fromArguments.Add((taskMethod, key, includeParameterlessMethodByDefault));
            if (!string.IsNullOrEmpty(help))
            {
                ArgumentHelp.Add(argKey, help);
            }
            else
            {
                var methodExpression = taskMethod.Body as MethodCallExpression;
                if (methodExpression != null)
                {
                    ArgumentHelp.Add(argKey, $"Pass argument '{argKey}' to method '{methodExpression.Method.Name}'.");
                }
            }

            return this as TTask;
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

        [DisableForMember]
        public TTask SetDescription(string description)
        {
            Description = description;
            return this as TTask;
        }
        
        /// <inheritdoc />
        [DisableForMember]
        public void ExecuteVoid(ITaskContext context)
        {
            Execute(context);
        }

        /// <inheritdoc />
        [DisableForMember]
        public async System.Threading.Tasks.Task ExecuteVoidAsync(ITaskContext context)
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
        [DisableForMember]
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
        [DisableForMember]
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
                    await System.Threading.Tasks.Task.Delay(RetryDelay);
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

        public override void LogTaskHelp(ITaskContext context)
        {
            context.LogInfo(" ");
            context.LogInfo(string.Empty);
            context.LogInfo($"{TaskName}: {Description}");
            if (ArgumentHelp == null || ArgumentHelp.Count <= 0)
            {
                return;
            }

            context.LogInfo(string.Empty);
            context.LogInfo("   Task arguments:");
            foreach (var argument in ArgumentHelp)
            {
                context.LogInfo($"        {argument.Key}    {argument.Value}");
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
            return await System.Threading.Tasks.Task.Run(() => DoExecute(context));
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

        private void InvokeFromMethods()
        {
            if (_fromArguments.Count == 0)
            {
                return;
            }

            foreach (var fromArgument in _fromArguments)
            {
                var methodCallExpression = fromArgument.TaskMethod.Body as MethodCallExpression;
                if (methodCallExpression != null)
                {
                    var attribute = methodCallExpression.Method.GetCustomAttribute<DisableForMemberAttribute>();

                    if (attribute != null)
                    {
                        throw new TaskExecutionException($"ForMember is not allowed on method '{methodCallExpression.Method.Name}'.", 20);
                    }
                }
                else
                {
                    continue;
                }

                if (!Context.ScriptArgs.ContainsKey(fromArgument.ArgKey))
                {
                    if (methodCallExpression.Arguments.Count == 0 && !fromArgument.includeParameterlessMethodByDefault)
                    {
                        return;
                    }

                    fromArgument.TaskMethod.Compile().Invoke(this as TTask);
                    return;
                }

                string value = Context.ScriptArgs[fromArgument.ArgKey];
                MethodParameterModifier parameterModifier = new MethodParameterModifier();
                try
                {
                    if (methodCallExpression.Arguments.Count == 0)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            value = "true";
                        }
                        
                        var succeded = bool.TryParse(value, out var boolValue);

                        if (succeded && boolValue)
                        {
                            fromArgument.TaskMethod.Compile().Invoke(this as TTask);
                        }

                        return;
                    }

                    var newExpression = (Expression<Action<TTask>>)parameterModifier.Modify(fromArgument.TaskMethod, new List<string>() { value });
                    var action = newExpression.Compile();
                    action.Invoke(this as TTask);
                }
                catch (FormatException e)
                {
                   var methodInfo = ((MethodCallExpression) fromArgument.TaskMethod.Body).Method;
                   var parameters =  methodInfo.GetParameters().ToList();
                    if (parameters.Count == 1)
                    {
                        throw new TaskExecutionException(
                            $"Parameter '{parameters[0].ParameterType.Name} {parameters[0].Name}' in method '{methodInfo.Name}' can not be modified with value '{value}' from argument '-{fromArgument.ArgKey}'.",
                            21, e);
                    }
                }
            }
        }
    }

    public abstract class TaskHelp
    {
        public abstract void LogTaskHelp(ITaskContext context);
    }
}