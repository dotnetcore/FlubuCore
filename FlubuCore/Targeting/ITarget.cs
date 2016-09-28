using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public interface ITarget : ITask
    {
        ICollection<string> Dependencies { get; }

        string TargetName { get; }

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

        ITarget Do(Action<ITaskContext> targetAction);

        /// <summary>
        ///     Overrides any previously specified target action with the new one.
        /// </summary>
        /// <param name="targetAction">The new target action to perform.</param>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        /// <remarks>The method works even if no target action was specified before.</remarks>
        ITarget OverrideDo(Action<ITaskContext> targetAction);

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITarget SetAsDefault();

        ITarget SetDescription(string description);

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="Targeting.ITarget" />.</returns>
        ITarget SetAsHidden();

        /// <summary>
        ///     Adds this target to target tree.
        /// </summary>
        /// <param name="targetTree">The <see cref="TargetTree" /> that target will be added to.</param>
        TargetTree AddToTargetTree(TargetTree targetTree);

        ITarget AddTask(params ITask[] tasks);
    }
}
