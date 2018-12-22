using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     Specifies basic properties and methods for a task.
    /// </summary>
    public interface ITaskOfT<T, TTask> : ITask
        where TTask : ITask
    {
        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        T Execute(ITaskContext context);

        Task<T> ExecuteAsync(ITaskContext context);

        /// <summary>
        /// Retry task if execution of the task fails.
        /// </summary>
        /// <param name="numberOfRetries">Number of retries before task fails.</param>
        /// <param name="delay">Delay time in miliseconds between retries.</param>
        /// <param name="condition">Condition when retry will occur. If condition is null task is always retried. </param>
        /// <returns></returns>
        TTask Retry(int numberOfRetries, int delay = 500, Func<ITaskContext, Exception, bool> condition = null);

        /// <summary>
        /// Do not log messages.
        /// </summary>
        /// <returns></returns>
        TTask NoLog();

        /// <summary>
        /// Do not fail task if error occurs.
        /// </summary>
        /// <returns></returns>
        /// <param name="doNotFailOnErrorAction">Action to be taken if task fails and DoNotFailOnError flag is set.</param>
        /// <param name="condition">Condition when task will not fail. If condition is null task will never fail. </param>
        TTask DoNotFailOnError(Action<Exception> doNotFailOnErrorAction = null, Func<ITaskContext, Exception, bool> condition = null);

        /// <summary>
        /// Action to be taken when task finisish successfuly or when it fails.
        /// </summary>
        /// <param name="finallyAction"></param>
        /// <returns></returns>
        TTask Finally(Action<ITaskContext> finallyAction, bool cleanupOnCancel = false);

        /// <summary>
        /// Action to be taken if task fails.
        /// </summary>
        /// <param name="onErrorAction"></param>
        /// <returns></returns>
        TTask OnError(Action<ITaskContext, Exception> onErrorAction);

        /// <summary>
        /// Passes argument value with specified key in <see cref="argKey"/> to specified method  in <see cref="taskMethod"/>
        /// </summary>
        /// <param name="taskMember">The method that the parameter value will be modified.</param>
        /// <param name="argKey">The key of the argument that it will be pass through to method parameter in <see cref="taskMethod"/></param>
        /// <param name="help">The argument help  text that wiil be display in target help. If not specified default help text is used.</param>
        /// <param name="includeParameterlessMethodByDefault">If <c>true</c> <see cref="taskMethod"/> is invoked by default if <see cref="argKey"/> is not specified. Oterwise <see cref="taskMethod"/> is not invoked by default.</param>
        /// <returns></returns>
        TTask ForMember(Expression<Func<TTask, object>> taskMember, string argKey, string help = null, bool includeParameterlessMethodByDefault = false);

        /// <summary>
        /// Overrides default task description.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        TTask SetDescription(string description);

        /// <summary>
        /// When applied task duration is logged.
        /// </summary>
        /// <returns></returns>
        TTask LogTaskDuration();
    }
}
