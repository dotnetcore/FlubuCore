using System;
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
        /// <param name="tasks">Array of <see cref="ITarget"/> to add.</param>
        /// <returns>This same instance of <see cref="FlubuCore.Targeting" />.</returns>
        TTargetFluentInterface AddTask(params ITask[] tasks);

        /// <summary>
        /// Add's the specified task to the target.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified .net core task to the target.
        /// </summary>
        /// <param name="task">The .net core task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified .net core task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task);

         /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface Do(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script. . U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface Do<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface Do<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1>(Func<ITaskContextInternal, T, T1, Task> targetAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContextInternal, T, T1, T2, Task> targetAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContextInternal, T, T1, T2, T3, Task> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContextInternal, T, T1, T2, T3, T4, Task> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null);
    }

    public interface ITargetBaseFluentInterface
    {
    }
}
