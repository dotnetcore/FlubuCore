using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase<TResult, TTask> : TaskHelp, ITaskOfT<TResult, TTask>
        where TTask : class, ITask
    {
        private readonly List<(Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool interactive)> _forMembers = new List<(Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool interactive)>();

        private int _retriedTimes;

        private string _taskName;

        private bool _cleanUpOnCancel = false;

        private Action<ITaskContext> _finallyAction;

        private Action<ITaskContext, Exception> _onErrorAction;

        private Func<ITaskContext, Exception, bool> _retryCondition;

        private Func<ITaskContext, Exception, bool> _doNotFailCondition;

        private Action<Exception> _doNotFailOnErrorAction;

        internal List<(string argumentKey, string help)> ArgumentHelp { get; } = new List<(string argumentKey, string help)>();

        protected internal override string TaskName
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

        /// <inheritdoc />
        public TTask NoLog()
        {
            DoNotLog = true;
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
            TaskExecuted = true;
            if (_cleanUpOnCancel)
            {
                CleanUpStore.AddCleanupAction(_finallyAction);
            }

            ITaskContextInternal contextInternal = (ITaskContextInternal)context;

            Context = context ?? throw new ArgumentNullException(nameof(context));
            TaskStopwatch.Start();

            try
            {
                if (contextInternal.Args.DryRun)
                {
                    return default(TResult);
                }

                InvokeForMembers(_forMembers, contextInternal.Args.DisableInteractive);
                return DoExecute(contextInternal);
            }
            catch (Exception ex)
            {
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

                    if (LogDuration)
                    {
                        DoLogInfo($"{TaskName} finished (took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)");
                    }
                }
            }
        }

        /// <inheritdoc />
        [DisableForMember]
        public async Task<TResult> ExecuteAsync(ITaskContext context)
        {
            TaskExecuted = true;
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;
            Context = context ?? throw new ArgumentNullException(nameof(context));

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

                return await DoExecuteAsync(contextInternal);
            }
            catch (Exception ex)
            {
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
                    return await ExecuteAsync(context);
                }

                if (DoNotFail)
                {
                    var shouldFail = _doNotFailCondition != null && !_doNotFailCondition.Invoke(Context, ex);
                    if (shouldFail)
                    {
                        throw;
                    }

                    contextInternal.LogInfo($"Task didn't complete succesfully. Continuing with task execution as parameter DoNotFail was set on this task. Exception: {ex.Message}");
                    _doNotFailOnErrorAction?.Invoke(ex);
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

                if (LogDuration)
                {
                    DoLogInfo($"{TaskName} finished (took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)");
                }
            }
        }

        protected internal override void LogTaskHelp(ITaskContext context)
        {
            context.LogInfo(" ");
            context.LogInfo(string.Empty);
            context.LogInfo($"    {TaskName}: {Description}");
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

        private void PassArgumentValueToProperty((Expression<Func<TTask, object>> Member, string ArgKey, string consoleText, bool includeParameterlessMethodByDefault, bool Interactive) forMember, MemberExpression memberExpression, bool disableInteractive)
        {
            var attribute = memberExpression.Member.GetCustomAttribute<DisableForMemberAttribute>();

            if (attribute != null)
            {
                throw new TaskExecutionException($"ForMember is not allowed on property '{memberExpression.Member.Name}'.", 20);
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
            catch (FormatException ex)
            {
                throw new TaskExecutionException(
                    $"Property '{propertyInfo.Name}' can not be modified with value '{value}' from argument '-{forMember.ArgKey}'.",
                    21,
                    ex);
            }
        }

        private MemberExpression GetMemberExpression(Expression<Func<TTask, object>> exp)
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
    }

    public abstract class TaskHelp
    {
        protected internal virtual string TaskName { get; set; }

        protected internal bool TaskExecuted { get;  set; }

        protected internal abstract void LogTaskHelp(ITaskContext context);
    }
}