using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITargetBaseFluentInterfaceOfT<TTargetFluentInterface> : ITargetBaseFluentInterface
        where TTargetFluentInterface : ITargetBaseFluentInterface
    {
        /// <summary>
        /// Add multiple tasks to the target.
        /// </summary>
        /// <param name="tasks">Array of <see cref="ITargetInternal"/> to add.</param>
        /// <returns>This same instance of <see cref="FlubuCore.Targeting" />.</returns>
        TTargetFluentInterface AddTask(params ITask[] tasks);

        /// <summary>
        /// Add's the specified task to the target.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified .net core task to the target.
        /// </summary>
        /// <param name="task">The .net core task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified .net core task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified target to this target.
        /// </summary>
        /// <param name="target">The target to be added.</param>
        /// <returns></returns>
        TTargetFluentInterface AddTarget(ITarget target);

        /// <summary>
        /// Add's the specified target to this target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        TTargetFluentInterface AddTargetAsync(ITarget target);

         /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface Do(Action<ITaskContext> doAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script. . U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface Do<T>(Action<ITaskContext, T> doAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface Do<T, T1>(Action<ITaskContext, T, T1> doAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface Do<T, T1, T2>(Action<ITaskContext, T, T1, T2> doAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContext, T, T1, T2, T3> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContext, T, T1, T2, T3, T4> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync(Action<ITaskContext> doAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync(Func<ITaskContext, Task> doAction, Action<DoTaskAsync> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T>(Action<ITaskContext, T> doAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T>(Func<ITaskContext, T, Task> doAction, T param, Action<DoTaskAsync2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1>(Action<ITaskContext, T, T1> doAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1>(Func<ITaskContext, T, T1, Task> doAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContext, T, T1, T2> doAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContext, T, T1, T2, Task> doAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContext, T, T1, T2, T3> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContext, T, T1, T2, T3, Task> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContext, T, T1, T2, T3, T4> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContext, T, T1, T2, T3, T4, Task> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Target executes action specified before when only if specified <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        TTargetFluentInterface When(Func<ITaskContext, bool> condition);

        /// <summary>
        /// Condition to be checked before target is executed. If <see cref="condition"/> is not meet execution of script fails.
        /// </summary>
        /// <param name="condition">The condition to be checked.</param>
        /// <param name="message">The message displayed in logs when condition is not meet. </param>
        /// <returns></returns>
        TTargetFluentInterface Must(Func<ITaskContext, bool> condition, string message = null);

        /// <summary>
        /// Iterates through <see cref="collection"/> and adds specified actions to target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The acction.</param>
        /// <returns></returns>
        TTargetFluentInterface ForEach<T>(IEnumerable<T> collection, Action<T, TTargetFluentInterface> action);

        /// <summary>
        /// Checks if specified parameter is null. If it is null target execution fails before any task is executed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        TTargetFluentInterface Requires<T>(Expression<Func<T>> parameter);

        /// <summary>
        /// Enables / disables sequential logging in asynchronous executed tasks and target dependencies.
        /// </summary>
        /// <returns></returns>
        TTargetFluentInterface SequentialLogging(bool enable);
    }

    public interface ITargetBaseFluentInterface
    {
    }
}
