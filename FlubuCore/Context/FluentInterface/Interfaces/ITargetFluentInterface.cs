using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITargetFluentInterface : ITargetBaseFluentInterfaceOfT<ITargetFluentInterface>
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
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2, T3, T4, T5, T6>(Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2, T3, T4, T5, T6, T7>(Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

        /// <summary>
        /// Adds set of tasks to target.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface Do<T, T2, T3, T4, T5, T6, T7, T8>(Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface> action);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T> action, T param);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2> action, T param, T2 param2);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2, T3>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3> action, T param, T2 param2, T3 param3);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2, T3, T4>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2, T3, T4, T5>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2, T3, T4, T5, T6>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2, T3, T4, T5, T6, T7>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

        /// <summary>
        /// Adds set of tasks to target if <see cref="condition"/> is meet.
        /// </summary>
        /// <param name="action">Define tasks to be added to target.</param>
        /// <returns></returns>
        ITargetFluentInterface When<T, T2, T3, T4, T5, T6, T7, T8>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

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

        /// <summary>
        /// Group tasks and perform specific action on tasks.
        /// </summary>
        /// <param name="targetAction">specify tasks in group.</param>
        /// <param name="onFinally">action that will be taken when all task finish or when error occures.</param>
        /// <param name="onError">action that will be taken on any task actions</param>
        /// <returns></returns>
        ITargetFluentInterface Group(Action<ITargetBaseFluentInterfaceOfT<ITargetFluentInterface>> targetAction, Action<ITaskContext> onFinally = null, Action<ITaskContext, Exception> onError = null);
    }
}
