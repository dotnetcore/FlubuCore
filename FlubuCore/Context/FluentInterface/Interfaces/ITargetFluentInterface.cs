using System;
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
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOn(params ITarget[] targets);

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface DependsOn(params ITargetFluentInterface[] targets);

        /// <summary>
        /// Execute custom code in script.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface Do(Action<ITaskContextInternal> targetAction);

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
        /// Add's the (core) task to the target that will be run asynchonusly with other tasks.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the .net core task to the target that will be run asynchrounusly with other tasks.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task);

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
