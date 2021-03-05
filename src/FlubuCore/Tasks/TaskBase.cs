using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using Microsoft.Build.Framework;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase<TResult, TTask> : TaskCore, ITaskOfT<TResult, TTask>
        where TTask : class, ITask
    {
        private static object _logLockObj = new object();

        private readonly List<(Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool interactive)> _forMembers = new List<(Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool interactive)>();

        private int _retriedTimes;

        private string _taskName;

        private bool _cleanUpOnCancel = false;

        private Action<ITaskContext> _finallyAction;

        private Action<ITaskContext, Exception> _onErrorAction;

        private Func<ITaskContext, Exception, bool> _retryCondition;

        private Func<ITaskContext, Exception, bool> _doNotFailCondition;

        private Action<Exception> _doNotFailOnErrorAction;

        private List<string> _sequentialLogs;

        protected TaskExecutionMode TaskExecutionMode { get; private set; }

        internal List<(string argumentKey, string help)> ArgumentHelp { get; } = new List<(string argumentKey, string help)>();

        public override string TaskName
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

            protected internal set => _taskName = value;
        }

        /// <summary>
        /// Stopwatch for timings.
        /// </summary>
        internal Stopwatch TaskStopwatch { get; } = new Stopwatch();

        protected abstract string Description { get; set; }

        /// <summary>
        ///  Should the task fail if an error occurs.
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
        protected ITaskContext Context { get; private set; }

        /// <summary>
        /// Sets the tasks log level.
        /// </summary>
        protected LogLevel TaskLogLevel { get; private set; } = LogLevel.Info;

        /// <summary>
        /// Number of retries in case of an exception.
        /// </summary>
        protected int NumberOfRetries { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the duration of the task should be logged after the task
        ///     has finished.
        /// </summary>
        /// <value><c>true</c> if duration should be logged; otherwise, <c>false</c>.</value>
        protected virtual bool LogDuration { get; set; } = false;

        /// <inheritdoc />
        [DisableForMember]
        public TTask DoNotFailOnError(Action<Exception> doNotFailOnErrorAction = null, Func<ITaskContext, Exception, bool> condition = null)
        {
            DoNotFail = true;
            _doNotFailOnErrorAction = doNotFailOnErrorAction;
            _doNotFailCondition = condition;
            return this as TTask;
        }

        [DisableForMember]
        public TTask Finally(Action<ITaskContext> finallyAction, bool cleanupOnCancel = false)
        {
            _finallyAction = finallyAction;
            _cleanUpOnCancel = cleanupOnCancel;
            return this as TTask;
        }

        [DisableForMember]
        public TTask OnError(Action<ITaskContext, Exception> onErrorAction)
        {
            _onErrorAction = onErrorAction;
            return this as TTask;
        }

        [DisableForMember]
        public TTask When(Func<bool> condition, Action<TTask> taskAction)
        {
            var task = this as TTask;
            var conditionMeet = condition?.Invoke() ?? true;

            if (conditionMeet)
            {
                taskAction.Invoke(task);
            }

            return task;
        }

        /// <inheritdoc />
        [Obsolete("Use `WithLogLevel(LogLevel.None)` instead")]
        public TTask NoLog()
        {
            TaskLogLevel = LogLevel.None;
            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithLogLevel(LogLevel logLevel)
        {
            TaskLogLevel = logLevel;
            return this as TTask;
        }

        [DisableForMember]
        public TTask ForMember(Expression<Func<TTask, object>> taskMember, string argKey, string help = null, bool includeParameterlessMethodByDefault = true)
        {
            string key = argKey.TrimStart('-');
            _forMembers.Add((taskMember, key, null, includeParameterlessMethodByDefault, false));
            if (!string.IsNullOrEmpty(help))
            {
                ArgumentHelp.Add((argKey, help));
            }
            else
            {
                GetDefaultArgumentHelp(taskMember, argKey);
            }

            return this as TTask;
        }

        [DisableForMember]
        public TTask Interactive(Expression<Func<TTask, object>> taskMember, string argKey = null, string consoleText = null, string argHelp = null)
        {
            string key = null;
            if (argKey != null)
            {
                key = argKey.TrimStart('-');
            }
            else
            {
                key = GetDefaultArgKeyFromMethodName(taskMember, key);
            }

            if (!string.IsNullOrEmpty(argHelp))
            {
                ArgumentHelp.Add((key, argHelp));
            }
            else
            {
                GetDefaultArgumentHelp(taskMember, argKey);
            }

            if (consoleText == null)
            {
                consoleText = $"Enter value for parameter '{key}': ";
            }

            _forMembers.Add((taskMember, key, consoleText, false, true));
            return this as TTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="numberOfRetries">Number of retries before task fails.</param>
        /// <param name="delay">Delay time in miliseconds between retries.</param>
        /// <param name="condition">Condition when retry will occur. If condition is null task is always retried. </param>
        /// <returns></returns>
        public TTask Retry(int numberOfRetries, int delay = 500, Func<ITaskContext, Exception, bool> condition = null)
        {
            DoRetry = true;
            NumberOfRetries = numberOfRetries;
            RetryDelay = delay;
            _retryCondition = condition;
            return this as TTask;
        }

        [DisableForMember]
        public TTask SetDescription(string description)
        {
            Description = description;
            return this as TTask;
        }

        public TTask LogTaskDuration()
        {
            LogDuration = true;
            return this as TTask;
        }

        public TTask DoNotLogTaskExecutionInfo()
        {
            LogTaskExecutionInfo = false;
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
        [DisableForMember]
        public TResult Execute(ITaskContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;
            TaskExecutionMode = TaskExecutionMode.Sync;
            _sequentialLogs = new List<string>();
            TaskExecuted = true;

            if (!IsTarget && LogTaskExecutionInfo)
            {
                contextInternal.DecreaseDepth();
                LogSequentially($"Executing task {TaskName}", Color.DimGray);
                contextInternal.IncreaseDepth();
            }

            if (_cleanUpOnCancel)
            {
                CleanUpStore.AddCleanupAction(_finallyAction);
            }

            TaskStopwatch.Start();

            try
            {
                if (contextInternal.Args.DryRun)
                {
                    return default(TResult);
                }

                InvokeForMembers(_forMembers, contextInternal.Args.DisableInteractive);
                var result = DoExecute(contextInternal);
                TaskStatus = TaskStatus.Succeeded;
                return result;
            }
            catch (Exception ex)
            {
                TaskStatus = TaskStatus.Failed;
                _onErrorAction?.Invoke(Context, ex);
                var shouldRetry = _retryCondition == null || _retryCondition.Invoke(Context, ex);

                if (!shouldRetry && DoNotFail)
                {
                    var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                    if (shouldFail)
                    {
                        throw;
                    }

                    contextInternal.LogInfo(
                        $"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                    _doNotFailOnErrorAction?.Invoke(ex);
                    TaskStatus = TaskStatus.Succeeded;
                    return default(TResult);
                }

                if (!DoRetry)
                {
                    if (DoNotFail)
                    {
                        var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                        if (shouldFail)
                        {
                            throw;
                        }

                        contextInternal.LogInfo(
                            $"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                        _doNotFailOnErrorAction?.Invoke(ex);
                        TaskStatus = TaskStatus.Succeeded;
                        return default(TResult);
                    }

                    throw;
                }
                else
                {
                    if (!shouldRetry && !DoNotFail)
                    {
                        throw;
                    }
                }

                while (_retriedTimes < NumberOfRetries)
                {
                    TaskStatus = TaskStatus.NotRan;
                    _retriedTimes++;
                    contextInternal.LogInfo($"Task failed: {ex.Message}");
                    DoLogInfo($"Retriying for {_retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    Thread.Sleep(RetryDelay);
                    return Execute(context);
                }

                if (DoNotFail)
                {
                    var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                    if (shouldFail)
                    {
                        throw;
                    }

                    contextInternal.LogInfo(
                        $"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                    _doNotFailOnErrorAction?.Invoke(ex);
                    TaskStatus = TaskStatus.Succeeded;
                    return default(TResult);
                }

                throw;
            }
            finally
            {
                if (!CleanUpStore.StoreAccessed)
                {
                    if (_cleanUpOnCancel)
                    {
                        CleanUpStore.RemoveCleanupAction(_finallyAction);
                    }

                    _finallyAction?.Invoke(context);

                    TaskStopwatch.Stop();
                    LogFinishedStatus();
                    LogSequentialLogs(Context);

                    if (TaskStatus == TaskStatus.Failed && IsTarget)
                    {
                        contextInternal.DecreaseDepth();
                    }
                }
            }
        }

        /// <inheritdoc />
        [DisableForMember]
        public async Task<TResult> ExecuteAsync(ITaskContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            TaskExecutionMode = TaskExecutionMode.Async;
            _sequentialLogs = new List<string>();
            TaskExecuted = true;
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;

            if (!IsTarget && LogTaskExecutionInfo)
            {
                contextInternal.DecreaseDepth();
                LogSequentially($"Executing task '{TaskName}' asynchronous.", Color.DimGray);
                contextInternal.IncreaseDepth();
            }

            TaskStopwatch.Start();

            if (_cleanUpOnCancel)
            {
                CleanUpStore.AddCleanupAction(_finallyAction);
            }

            try
            {
                if (contextInternal.Args.DryRun)
                {
                    return default(TResult);
                }

                InvokeForMembers(_forMembers, contextInternal.Args.DisableInteractive);

                var result = await DoExecuteAsync(contextInternal);
                TaskStatus = TaskStatus.Succeeded;
                return result;
            }
            catch (Exception ex)
            {
                 TaskStatus = TaskStatus.Failed;
                _onErrorAction?.Invoke(Context, ex);
                var shouldRetry = _retryCondition == null || _retryCondition.Invoke(Context, ex);

                if (!shouldRetry && DoNotFail)
                {
                    var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                    if (shouldFail)
                    {
                        throw;
                    }

                    DoLogInfo($"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                    _doNotFailOnErrorAction?.Invoke(ex);
                    TaskStatus = TaskStatus.Succeeded;
                    return default(TResult);
                }

                if (!DoRetry)
                {
                    if (DoNotFail)
                    {
                        var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                        if (shouldFail)
                        {
                            throw;
                        }

                        DoLogInfo($"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                        _doNotFailOnErrorAction?.Invoke(ex);
                        TaskStatus = TaskStatus.Succeeded;
                        return default(TResult);
                    }

                    throw;
                }
                else
                {
                    if (!shouldRetry && !DoNotFail)
                    {
                        throw;
                    }
                }

                while (_retriedTimes < NumberOfRetries)
                {
                    _retriedTimes++;
                    contextInternal.LogInfo($"Task failed: {ex.Message}");
                    DoLogInfo($"Retriying for {_retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    await Task.Delay(RetryDelay);
                    TaskStatus = TaskStatus.NotRan;
                    return await ExecuteAsync(context);
                }

                if (DoNotFail)
                {
                    var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                    if (shouldFail)
                    {
                        throw;
                    }

                    DoLogInfo($"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                    _doNotFailOnErrorAction?.Invoke(ex);
                    TaskStatus = TaskStatus.Succeeded;
                    return default(TResult);
                }

                throw;
            }
            finally
            {
                if (!CleanUpStore.StoreAccessed)
                {
                    if (_cleanUpOnCancel)
                    {
                        CleanUpStore.RemoveCleanupAction(_finallyAction);
                    }

                    _finallyAction?.Invoke(context);
                }

                TaskStopwatch.Stop();
                LogFinishedStatus();
                LogSequentialLogs(Context);

                if (TaskStatus == TaskStatus.Failed && IsTarget)
                {
                    contextInternal.DecreaseDepth();
                }
            }
        }

        internal void LogFinishedStatus()
        {
            var statusMessage = StatusMessage();
            var duration = TaskStatus == TaskStatus.Succeeded || TaskStatus == TaskStatus.Failed
                ? $"(took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)"
                : string.Empty;

            if (LogDuration)
            {
                DoLogInfo($"{TaskName} {statusMessage} {duration}", Color.DimGray);
            }
        }

        protected internal override void LogTaskHelp(ITaskContext context)
        {
            context.LogInfo(" ");
            context.LogInfo(string.Empty);
            context.LogInfo($"    {TaskName.Capitalize()}: {Description}");
            if (ArgumentHelp == null || ArgumentHelp.Count <= 0)
            {
                return;
            }

            context.LogInfo(string.Empty);
            context.LogInfo("       Task arguments:");
            context.LogInfo(" ");
            foreach (var argument in ArgumentHelp)
            {
                context.LogInfo($"        -{argument.argumentKey}    {argument.help}");
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

        /// <summary>
        /// Log info if task logging is not disabled.
        /// </summary>
        /// <param name="message"></param>
        protected void DoLogInfo(string message)
        {
            if (TaskLogLevel < LogLevel.Info)
                return;

            LogSequentially(message);
        }

        /// <summary>
        /// Log info if task logging is not disabled.
        /// </summary>
        /// <param name="message"></param>
        protected void DoLogInfo(string message, Color foregroundColor)
        {
            if (TaskLogLevel < LogLevel.Info)
                return;

            LogSequentially(message, foregroundColor);
        }

        protected void LogSequentially(string message, Color foregroundColor)
        {
            if (SequentialLogging)
            {
                _sequentialLogs.Add(message);
            }
            else
            {
                Context.LogInfo(message, foregroundColor);
            }
        }

        /// <summary>
        /// Log error if task logging is not disabled.
        /// </summary>
        /// <param name="message"></param>
        protected void DoLogError(string message)
        {
            if (TaskLogLevel < LogLevel.Error)
                return;

            LogErrorSequentially(message);
        }

        /// <summary>
        /// Log error if task logging is not disabled.
        /// </summary>
        /// <param name="message"></param>
        protected void DoLogError(string message, Color foregroundColor)
        {
            if (TaskLogLevel < LogLevel.Error)
                return;

            if (SequentialLogging)
            {
                _sequentialLogs.Add(message);
            }
            else
            {
                Context.LogError(message, foregroundColor);
            }
        }

        protected void LogSequentially(string message)
        {
            if (SequentialLogging)
            {
                _sequentialLogs.Add(message);
            }
            else
            {
                Context.LogInfo(message);
            }
        }

        protected void LogErrorSequentially(string message)
        {
            if (SequentialLogging)
            {
                _sequentialLogs.Add(message);
            }
            else
            {
                Context.LogError(message);
            }
        }

        protected VSSolution GetRequiredVSSolution()
        {
            var solution = Context.Properties.TryGet<VSSolution>(BuildProps.Solution);

            if (solution == null)
            {
                throw new KeyNotFoundException($"Task context property VsSolution must be set for task {TaskName}. Execute LoadSolutionTask before this task.");
            }

            return solution;
        }

        private void InvokeForMembers(List<(Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool interactive)> members, bool disableInteractive)
        {
            if (members.Count == 0)
            {
                return;
            }

            foreach (var member in members)
            {
                var memberExpression = GetMemberExpression(member.Member);
                if (memberExpression != null)
                {
                    PassArgumentValueToProperty(member, memberExpression, disableInteractive);
                    continue;
                }

                var methodCallExpression = member.Member.Body as MethodCallExpression;
                if (methodCallExpression == null)
                {
                    continue;
                }

                PassArgumentValueToMethodParameter(member, methodCallExpression, disableInteractive);
            }
        }

        private void PassArgumentValueToMethodParameter((Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool interactive) forMember, MethodCallExpression methodCallExpression, bool disableInteractive)
        {
            var attribute = methodCallExpression.Method.GetCustomAttribute<DisableForMemberAttribute>();

            if (attribute != null)
            {
                throw new TaskExecutionException($"ForMember is not allowed on method '{methodCallExpression.Method.Name}'.", 20);
            }

            if (!Context.ScriptArgs.ContainsKey(forMember.ArgKey) && (!forMember.interactive || disableInteractive))
            {
                if (methodCallExpression.Arguments.Count == 0 && !forMember.includeParameterlessMethodByDefault)
                {
                    return;
                }

                forMember.Member.Compile().Invoke(this as TTask);
                return;
            }

            string argumentValue = Context.ScriptArgs[forMember.ArgKey];
            if (string.IsNullOrEmpty(argumentValue) && forMember.interactive)
            {
                if (!disableInteractive)
                {
                    Console.Write(forMember.consoleText);
                    argumentValue = Console.ReadLine();
                }
            }

            try
            {
                if (methodCallExpression.Arguments.Count == 0)
                {
                    if (string.IsNullOrEmpty(argumentValue))
                    {
                        forMember.Member.Compile().Invoke(this as TTask);
                        return;
                    }

                    var succeded = bool.TryParse(argumentValue, out var boolValue);

                    if (succeded)
                    {
                        if (boolValue)
                        {
                            forMember.Member.Compile().Invoke(this as TTask);
                        }
                    }
                    else
                    {
                        if (forMember.includeParameterlessMethodByDefault)
                        {
                            forMember.Member.Compile().Invoke(this as TTask);
                        }
                    }

                    return;
                }

                MethodParameterModifier parameterModifier = new MethodParameterModifier();
                var newExpression = (Expression<Func<TTask, object>>)parameterModifier.Modify(forMember.Member, new List<string> { argumentValue });
                newExpression.Compile().Invoke(this as TTask);
            }
            catch (FormatException e)
            {
                var methodInfo = ((MethodCallExpression)forMember.Member.Body).Method;
                var parameters = methodInfo.GetParameters().ToList();

                if (parameters.Count == 1)
                {
                    throw new TaskExecutionException(
                        $"Parameter '{parameters[0].ParameterType.Name} {parameters[0].Name}' in method '{methodInfo.Name}' can not be modified with value '{argumentValue}' from argument '-{forMember.ArgKey}'.",
                        21,
                        e);
                }
            }
        }

        private void PassArgumentValueToProperty(
            (Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool
                includeParameterlessMethodByDefault, bool Interactive) forMember, MemberExpression memberExpression,
            bool disableInteractive)
        {
            var attribute = memberExpression.Member.GetCustomAttribute<DisableForMemberAttribute>();

            if (attribute != null)
            {
                throw new TaskExecutionException(
                    $"ForMember is not allowed on property '{memberExpression.Member.Name}'.", 20);
            }

            if (!Context.ScriptArgs.ContainsKey(forMember.ArgKey) && (!forMember.Interactive || disableInteractive))
            {
                return;
            }

            string value = Context.ScriptArgs[forMember.ArgKey];

            if (string.IsNullOrEmpty(value) && forMember.Interactive)
            {
                if (!disableInteractive)
                {
                    Console.Write(forMember.consoleText);
                    value = Console.ReadLine();
                }
            }

            var propertyInfo = (PropertyInfo)memberExpression.Member;

            try
            {
                object parsedValue = MethodParameterModifier.ParseValueByType(value, propertyInfo.PropertyType);
                propertyInfo.SetValue(this, parsedValue, null);
            }
            catch (FormatException e)
            {
                throw new ScriptException(
                    $"Property '{propertyInfo.Name}' can not be modified with value '{value}' from argument '-{forMember.ArgKey}'.",
                    e);
            }
            catch (ArgumentException e)
            {
                throw new ScriptException(
                    $"Property '{propertyInfo.Name}' can not be modified with value '{value}' from argument '-{forMember.ArgKey}'.",
                    e);
            }
        }

        internal MemberExpression GetMemberExpression(Expression<Func<TTask, object>> exp)
        {
            var member = exp.Body as MemberExpression;
            var unary = exp.Body as UnaryExpression;
            return member ?? (unary != null ? unary.Operand as MemberExpression : null);
        }

        internal MemberExpression GetMemberExpression(LambdaExpression exp)
        {
            var member = exp.Body as MemberExpression;
            var unary = exp.Body as UnaryExpression;
            return member ?? (unary != null ? unary.Operand as MemberExpression : null);
        }

        private void GetDefaultArgumentHelp(Expression<Func<TTask, object>> taskMember, string argKey)
        {
            if (taskMember.Body is MethodCallExpression methodExpression)
            {
                string defaultValue = methodExpression.Arguments.Count == 1
                    ? $"Default value: '{methodExpression.Arguments[0]}'."
                    : null;
                ArgumentHelp.Add((argKey,
                    $"Pass argument with key '{argKey}' to method '{methodExpression.Method.Name}'. {defaultValue}"));
            }

            var propertyExpression = GetMemberExpression(taskMember);
            if (propertyExpression != null)
            {
                ArgumentHelp.Add((argKey,
                    $"Pass argument with key '{argKey}' to property '{propertyExpression.Member.Name}'."));
            }
        }

        private string GetDefaultArgKeyFromMethodName(Expression<Func<TTask, object>> taskMember, string key)
        {
            if (taskMember.Body is MethodCallExpression methodExpression)
            {
                key = methodExpression.Method.Name;
            }

            var propertyExpression = GetMemberExpression(taskMember);
            if (propertyExpression != null)
            {
                key = propertyExpression.Member.Name;
            }

            return key;
        }

        private void LogSequentialLogs(ITaskContext context)
        {
            if (_sequentialLogs == null || _sequentialLogs.Count == 0)
            {
                return;
            }

            lock (_logLockObj)
            {
                foreach (var log in _sequentialLogs)
                {
                    context.LogInfo(log);
                }
            }
        }

        private string StatusMessage()
        {
            string statusMessage;
            switch (TaskStatus)
            {
                case TaskStatus.Succeeded:
                {
                    statusMessage = "succeeded";
                    break;
                }

                case TaskStatus.Failed:
                {
                    statusMessage = "failed";
                    break;
                }

                case TaskStatus.Skipped:
                {
                    statusMessage = "skipped";
                    break;
                }

                case TaskStatus.NotRan:
                {
                    statusMessage = "not ran";
                    break;
                }

                default:
                    throw new NotSupportedException("Task status not supported.");
            }

            return statusMessage;
        }
    }

    public abstract class TaskCore
    {
        public virtual string TaskName { get; protected internal set; }

        public TaskStatus TaskStatus { get; protected set; } = TaskStatus.NotRan;

        public bool SequentialLogging { get; set; }

        public virtual bool IsTarget { get; } = false;

        public bool LogTaskExecutionInfo { get; set; } = true;

        protected internal bool TaskExecuted { get;  set; }

        protected internal abstract void LogTaskHelp(ITaskContext context);
    }
}