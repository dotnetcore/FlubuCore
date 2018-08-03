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
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do(Action<ITarget> action);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T>(Action<ITarget, T> action, T param);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2>(Action<ITarget, T, T2> action, T param, T2 param2);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2, T3>(Action<ITarget, T, T2, T3> action, T param, T2 param2, T3 param3);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2, T3, T4>(Action<ITarget, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2, T3, T4, T5>(Action<ITarget, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2, T3, T4, T5, T6>(Action<ITarget, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2, T3, T4, T5, T6, T7>(Action<ITarget, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        [Obsolete("Do method for adding set of tasks was renamed to AddTasks.")]
        ITarget Do<T, T2, T3, T4, T5, T6, T7, T8>(Action<ITarget, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

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
        /// <param name="onError">action that will be taken on any task actions</param>
        /// <param name="when">Tasks will be added only if specified condition is meet.</param>
        /// <returns></returns>
        ITarget Group(Action<ITargetBaseFluentInterfaceOfT<ITarget>> targetAction, Action<ITaskContext> onFinally = null, Action<ITaskContext, Exception> onError = null, Func<ITaskContext, bool> when = null);
    }
}
