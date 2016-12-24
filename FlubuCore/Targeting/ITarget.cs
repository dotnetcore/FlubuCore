using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public interface ITarget : ITask
    {
        ICollection<string> Dependencies { get; }

        string TargetName { get; }

        string Description { get; }

        /// <summary>
        ///     Gets a value indicating whether this target is hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        bool IsHidden { get; }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITarget DependsOn(params string[] targetNames);

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency target names.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITarget DependsOn(params ITarget[] targets);

        /// <summary>
        /// Execute custom code in script.
        /// </summary>
        /// <param name="targetAction">Action to execute.</param>
        /// <returns>This target.</returns>
        ITarget Do(Action<ITaskContextInternal> targetAction);

        /// <summary>
        ///     Overrides any previously specified target action with the new one.
        /// </summary>
        /// <param name="targetAction">The new target action to perform.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        /// <remarks>The method works even if no target action was specified before.</remarks>
        ITarget OverrideDo(Action<ITaskContextInternal> targetAction);

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITarget SetAsDefault();

        /// <summary>
        /// Set's the description of the target.
        /// Desciption will be displayed in help.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>this target</returns>
        ITarget SetDescription(string description);

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITarget SetAsHidden();

        /// <summary>
        /// Add's the task to the target.
        /// </summary>
        /// <param name="task">The task to be added</param>
        ITarget AddTask(params ITask[] task);
    }
}
