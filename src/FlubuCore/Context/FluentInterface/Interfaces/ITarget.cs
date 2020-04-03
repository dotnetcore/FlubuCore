using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITarget : ITargetBaseFluentInterfaceOfT<ITarget>
    {
         /// <summary>
        /// Add multiple tasks to the target.
        /// </summary>
        /// <param name="tasks">Array of <see cref="ITargetInternal"/> to add.</param>
        /// <returns>This same instance of <see cref="FlubuCore.Targeting" />.</returns>
        new ITarget AddTask(params ITask[] tasks);

        /// <summary>
        /// Add's the specified task to the target.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget AddTask(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified .net core task to the target.
        /// </summary>
        /// <param name="task">The .net core task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget AddTaskAsync(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the specified .net core task to the target that will be run asynchronous with other tasks.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task);

         /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget Do(Action<ITaskContext> doAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script. . U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget Do<T>(Action<ITaskContext, T> doAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameter.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget Do<T, T1>(Action<ITaskContext, T, T1> doAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget Do<T, T1, T2>(Action<ITaskContext, T, T1, T2> doAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget Do<T, T1, T2, T3>(Action<ITaskContext, T, T1, T2, T3> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget Do<T, T1, T2, T3, T4>(Action<ITaskContext, T, T1, T2, T3, T4> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync(Action<ITaskContext> doAction, Action<DoTask> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync(Func<ITaskContext, Task> doAction, Action<DoTaskAsync> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T>(Action<ITaskContext, T> doAction, T param, Action<DoTask2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T>(Func<ITaskContext, T, Task> doAction, T param, Action<DoTaskAsync2<T>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1>(Action<ITaskContext, T, T1> doAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1>(Func<ITaskContext, T, T1, Task> doAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1, T2>(Action<ITaskContext, T, T1, T2> doAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1, T2>(Func<ITaskContext, T, T1, T2, Task> doAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1, T2, T3>(Action<ITaskContext, T, T1, T2, T3> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1, T2, T3>(Func<ITaskContext, T, T1, T2, T3, Task> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1, T2, T3, T4>(Action<ITaskContext, T, T1, T2, T3, T4> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        /// Execute custom code in script asynchronous. U can invoke base task actions through optional parameters.
        /// </summary>
        /// <param name="doAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        new ITarget DoAsync<T, T1, T2, T3, T4>(Func<ITaskContext, T, T1, T2, T3, T4, Task> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null);

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget DependsOn(params string[] targetNames);

        /// <summary>
        ///     Specifies targets on which this target depends on. Execution of dependant targets is synchronus.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget DependsOn(params ITargetInternal[] targets);

        /// <summary>
        ///     Specifies targets on which this target depends on. Execution of dependant targets is asynchronus.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget DependsOnAsync(params ITargetInternal[] targets);

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget DependsOn(params ITarget[] targets);

        /// <summary>
        /// Specifies targets on which this target depends on. Execution of dependant targets is asynchronus.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget DependsOnAsync(params ITarget[] targets);

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget SetAsDefault();

        ITarget SetDescription(string description);

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        ITarget SetAsHidden();

         /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks(Action<ITarget> action);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T>(Action<ITarget, T> action, T param);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2>(Action<ITarget, T, T2> action, T param, T2 param2);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2, T3>(Action<ITarget, T, T2, T3> action, T param, T2 param2, T3 param3);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2, T3, T4>(Action<ITarget, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2, T3, T4, T5>(Action<ITarget, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2, T3, T4, T5, T6>(Action<ITarget, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2, T3, T4, T5, T6, T7>(Action<ITarget, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITarget AddTasks<T, T2, T3, T4, T5, T6, T7, T8>(Action<ITarget, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

        /// <summary>
        /// Group tasks and perform various actions(onFinally, onError, when) on tasks.
        /// </summary>
        /// <param name="targetAction">specify tasks to add in target action.</param>
        /// <param name="onFinally">action that will be taken when all task finish or when error occures.</param>
        /// <param name="onError">action that will be taken on any task actions.</param>
        /// <param name="when">Tasks will be added only if specified condition is meet.</param>
        /// <returns></returns>
        ITarget Group(Action<ITargetBaseFluentInterfaceOfT<ITarget>> targetAction, Action<ITaskContext> onFinally = null, Action<ITaskContext, Exception> onError = null, Func<ITaskContext, bool> when = null, bool cleanupOnCancel = false);
    }
}
