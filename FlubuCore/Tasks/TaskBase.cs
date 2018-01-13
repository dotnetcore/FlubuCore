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
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     A base abstract class from which tasks can be implemented.
    /// </summary>
    public abstract class TaskBase<TResult, TTask> : TaskHelp, ITaskOfT<TResult, TTask>
        where TTask : class, ITask
    {
        private readonly List<(Expression<Func<TTask, object>> Member, string ArgKey, bool includeParameterlessMethodByDefault)> _forMembers = new List<(Expression<Func<TTask, object>> Member, string ArgKey, bool includeParameterlessMethodByDefault)>();

        private int _retriedTimes;
        private string _taskName;

        internal List<(string argumentKey, string help)> ArgumentHelp { get; } = new List<(string argumentKey, string help)>();

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
        public TTask ForMember(Expression<Func<TTask, object>> taskMember, string argKey, string help = null, bool includeParameterlessMethodByDefault = true)
        {
            string key = argKey.TrimStart('-');
            _forMembers.Add((taskMember, key, includeParameterlessMethodByDefault));
            if (!string.IsNullOrEmpty(help))
            {
                ArgumentHelp.Add((argKey, help));
            }
            else
            {
                if (taskMember.Body is MethodCallExpression methodExpression)
                {
                    string defaultValue = methodExpression.Arguments.Count == 1
                        ? $"Default value: '{methodExpression.Arguments[0]}'."
                        : null;
                    ArgumentHelp.Add((argKey, $"Pass argument '{argKey}' to method '{methodExpression.Method.Name}'. {defaultValue}"));
                }

                var propertyExpression = GetMemberExpression(taskMember);
                if (propertyExpression != null)
                {
                    ArgumentHelp.Add((argKey, $"Pass argument '{argKey}' to property '{propertyExpression.Member.Name}'."));
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
            ITaskContextInternal contextInternal = (ITaskContextInternal)context;

            Context = context ?? throw new ArgumentNullException(nameof(context));
            TaskStopwatch.Start();

            if (!string.IsNullOrEmpty(DescriptionForLog))
            {
                DoLogInfo(DescriptionForLog);
            }

            try
            {
                InvokeForMembers();
                return DoExecute(contextInternal);
            }
            catch (Exception ex)
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
                    contextInternal.LogInfo($"Task failed: {ex.Message}");
                    contextInternal.LogInfo($"Retriying for {_retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
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

            try
            {
                InvokeForMembers();
                return await DoExecuteAsync(contextInternal);
            }
            catch (Exception ex)
            {
                if (!DoRetry)
                {
                    throw;
                }

                while (_retriedTimes < NumberOfRetries)
                {
                    _retriedTimes++;
                    contextInternal.LogInfo($"Task failed: {ex.Message}");
                    contextInternal.LogInfo($"Retriying for {_retriedTimes} time(s). Number of all retries {NumberOfRetries}.");
                    await Task.Delay(RetryDelay);
                    return await ExecuteAsync(context);
                }

                throw;
            }
            finally
            {
                TaskStopwatch.Stop();

                if (LogDuration)
                {
                    contextInternal.LogInfo($"{DescriptionForLog} finished (took {(int)TaskStopwatch.Elapsed.TotalSeconds} seconds)");
                }
            }
        }

        internal override void LogTaskHelp(ITaskContext context)
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

        private void InvokeForMembers()
        {
            if (_forMembers.Count == 0)
            {
                return;
            }

            foreach (var forMember in _forMembers)
            {
                var memberExpression = GetMemberExpression(forMember.Member);
                if (memberExpression != null)
                {
                    PassArgumentValueToProperty(forMember, memberExpression);
                    continue;
                }

                var methodCallExpression = forMember.Member.Body as MethodCallExpression;
                if (methodCallExpression == null)
                {
                    continue;
                }

                PassArgumentValueToMethodParameter(forMember, methodCallExpression);
            }
        }

        private void PassArgumentValueToMethodParameter((Expression<Func<TTask, object>> Member, string ArgKey, bool includeParameterlessMethodByDefault) forMember, MethodCallExpression methodCallExpression)
        {
            var attribute = methodCallExpression.Method.GetCustomAttribute<DisableForMemberAttribute>();

            if (attribute != null)
            {
                throw new TaskExecutionException($"ForMember is not allowed on method '{methodCallExpression.Method.Name}'.", 20);
            }

            if (!Context.ScriptArgs.ContainsKey(forMember.ArgKey))
            {
                if (methodCallExpression.Arguments.Count == 0 && !forMember.includeParameterlessMethodByDefault)
                {
                    return;
                }

                forMember.Member.Compile().Invoke(this as TTask);
                return;
            }

            string argumentValue = Context.ScriptArgs[forMember.ArgKey];

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

        private void PassArgumentValueToProperty((Expression<Func<TTask, object>> TaskMethod, string ArgKey, bool includeParameterlessMethodByDefault) forMember, MemberExpression memberExpression)
        {
            var attribute = memberExpression.Member.GetCustomAttribute<DisableForMemberAttribute>();

            if (attribute != null)
            {
                throw new TaskExecutionException($"ForMember is not allowed on property '{memberExpression.Member.Name}'.", 20);
            }

            if (!Context.ScriptArgs.ContainsKey(forMember.ArgKey))
            {
                return;
            }

            string value = Context.ScriptArgs[forMember.ArgKey];
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
    }

    public abstract class TaskHelp
    {
        internal abstract void LogTaskHelp(ITaskContext context);
    }
}