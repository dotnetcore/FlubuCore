using flubu.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace flubu.Targeting
{
    public interface ITarget : ITask
    {
        ICollection<string> Dependencies { get; }

        string TargetName { get; }

        /// <summary>
        /// Gets a value indicating whether this target is hidden. Hidden targets will not be
        /// visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        bool IsHidden { get; }

        /// <summary>
        /// Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        ITarget DependsOn(params string[] targetNames);

        /// <summary>
        /// Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency targets</param>
        /// <returns>This same instance of <see cref="ITarget"/></returns>
        ITarget DependsOn(params ITarget[] targets);

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Do")]
        ITarget Do(Action<ITaskContext> targetAction);
        
        /// <summary>
        /// Overrides any previously specified target action with the new one.
        /// </summary>
        /// <param name="targetAction">The new target action to perform.</param>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        /// <remarks>The method works even if no target action was specified before.</remarks>
        ITarget OverrideDo (Action<ITaskContext> targetAction);

        /// <summary>
        /// Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        ITarget SetAsDefault();

        ITarget SetDescription(string description);

        /// <summary>
        /// Sets the target as hidden. Hidden targets will not be
        /// visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        ITarget SetAsHidden ();
    }
}