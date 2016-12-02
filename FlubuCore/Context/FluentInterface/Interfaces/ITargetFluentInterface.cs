using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITargetFluentInterface
    {
        ITarget Target { get; set; }

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
        /// Add's the task to the target.
        /// </summary>
        /// <param name="task">The task to be added</param>
        /// <returns></returns>
        ITargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task);

        /// <summary>
        /// Add's the .net core task to the target.
        /// </summary>
        /// <param name="task">The .net core task to be added</param>
        /// <returns></returns>
        ITargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task);

        /// <summary>
        /// Task extensions for various tasks(Fluent interface).
        /// </summary>
        /// <returns></returns>
        ITaskExtensionsFluentInterface TaskExtensions();
    }
}
