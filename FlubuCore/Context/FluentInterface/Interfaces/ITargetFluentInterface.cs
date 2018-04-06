using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITargetFluentInterface
    {
        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOn(params string[] targetNames);

        /// <summary>
        ///     Specifies targets on which this target depends on. Execution of dependant targets is synchronus.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOn(params ITarget[] targets);

        /// <summary>
        ///     Specifies targets on which this target depends on. Execution of dependant targets is asynchronus.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOnAsync(params ITarget[] targets);

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOn(params ITargetFluentInterface[] targets);

        /// <summary>
        /// Specifies targets on which this target depends on. Execution of dependant targets is asynchronus.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOnAsync(params ITargetFluentInterface[] targets);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script. . U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1>(Func<ITaskContextInternal, T, T1, Task> targetAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContextInternal, T, T1, T2, Task> targetAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContextInternal, T, T1, T2, T3, Task> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContextInternal, T, T1, T2, T3, T4, Task> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface SetAsDefault();

        ITargetFluentInterface SetDescription(string description);

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface SetAsHidden();

        /// <summary>
        /// Add multiple tasks to the target.
        /// </summary>
        /// <param name="tasks">Array of <see cref="ITask"/> to add.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddTask(params ITask[] tasks);

        /// <summary>
        /// Add's the (core) task to the target.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the .net core task to the target.
        /// </summary>
        /// <param name="task">The .net core task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the (core) task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the .net core task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do(Action<ITargetFluentInterface> action);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T>(Action<ITargetFluentInterface, T> action, T param);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2>(Action<ITargetFluentInterface, T, T2> action, T param, T2 param2);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2, T3>(Action<ITargetFluentInterface, T, T2, T3> action, T param, T2 param2, T3 param3);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2, T3, T4>(Action<ITargetFluentInterface, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2, T3, T4, T5>(Action<ITargetFluentInterface, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5);

        /// <summary>
        /// Task extensions for various .net (core) tasks(Fluent interface).
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITaskExtensionsFluentInterface TaskExtensions();

        /// <summary>
        /// Task extensions for various .net core tasks(Fluent interface).
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ICoreTaskExtensionsFluentInterface CoreTaskExtensions();
    }
}
