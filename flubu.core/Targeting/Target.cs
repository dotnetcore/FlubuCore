using System;
using System.Collections.Generic;
using Flubu.Tasks;

namespace Flubu.Targeting
{
    public class Target : TaskBase, ITarget
    {
        private readonly List<string> dependencies = new List<string>();
        private string description;
        private Action<ITaskContext> targetAction;
        private TargetTree targetTree;

        public Target(string targetName)
        {
            TargetName = targetName;
        }

        public Target(TargetTree targetTree, string targetName)
        {
            this.targetTree = targetTree;
            TargetName = targetName;
        }

        public ICollection<string> Dependencies
        {
            get { return dependencies; }
        }

        /// <summary>
        ///     Gets the description of the target.
        /// </summary>
        /// <value>The description of the target.</value>
        public override string Description
        {
            get { return description; }
        }

        /// <summary>
        ///     Gets a value indicating whether this target is hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; private set; }

        public string TargetName { get; }

        protected override bool LogDuration
        {
            get { return true; }
        }

        protected override string DescriptionForLog
        {
            get { return TargetName; }
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget DependsOn(params string[] targetNames)
        {
            foreach (var dependentTargetName in targetNames)
            {
                dependencies.Add(dependentTargetName);
            }

            return this;
        }

        public ITarget Do(Action<ITaskContext> targetAction)
        {
            if (this.targetAction != null)
            {
                throw new ArgumentException("Target action was already set.");
            }

            this.targetAction = targetAction;
            return this;
        }

        public ITarget OverrideDo(Action<ITaskContext> targetAction)
        {
            this.targetAction = targetAction;
            return this;
        }

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget SetAsDefault()
        {
            targetTree.SetDefaultTarget(this);
            return this;
        }

        /// <summary>
        ///     Set's the description of the target,
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>this target</returns>
        public ITarget SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget SetAsHidden()
        {
            IsHidden = true;
            return this;
        }

        /// <summary>
        ///     Adds this target to target tree.
        /// </summary>
        /// <param name="targetTree">The <see cref="TargetTree" /> that target will be added to.</param>
        public void AddToTargetTree(TargetTree targetTree)
        {
            this.targetTree = targetTree;
            targetTree.AddTarget(this);
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency targets</param>
        /// <returns>This same instance of <see cref="ITarget" /></returns>
        public ITarget DependsOn(params ITarget[] targets)
        {
            foreach (var target in targets)
            {
                dependencies.Add(target.TargetName);
            }

            return this;
        }

        protected override void DoExecute(ITaskContext context)
        {
            if (targetTree == null)
            {
                throw new ArgumentNullException("targetTree", "TargetTree must be set before Execution of target.");
            }

            targetTree.MarkTargetAsExecuted(this);
            targetTree.EnsureDependenciesExecuted(context, TargetName);

            // we can have action-less targets (that only depend on other targets)
            targetAction?.Invoke(context);
        }
    }
}