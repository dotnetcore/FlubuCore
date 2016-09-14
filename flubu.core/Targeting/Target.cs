using flubu.Targeting;
using flubu.Tasks;
using System;
using System.Collections.Generic;

namespace flubu.Targeting
{
    public class Target : TaskBase, ITarget
    {
        public Target(TargetTree targetTree, string targetName)
        {
            this.targetTree = targetTree;
            this.targetName = targetName;
        }

        public ICollection<string> Dependencies
        {
            get { return dependencies; }
        }

        /// <summary>
        /// Gets the description of the target.
        /// </summary>
        /// <value>The description of the target.</value>
        public override string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets a value indicating whether this target is hidden. Hidden targets will not be
        /// visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get { return isHidden; }
        }

        public string TargetName
        {
            get { return targetName; }
        }

        /// <summary>
        /// Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        public ITarget DependsOn(params string[] targetNames)
        {
            foreach (string dependentTargetName in targetNames)
                dependencies.Add(dependentTargetName);
            return this;
        }

        public ITarget Do (Action<ITaskContext> targetAction)
        {
            if (this.targetAction != null)
                throw new ArgumentException("Target action was already set.");

            this.targetAction = targetAction;
            return this;
        }

        public ITarget OverrideDo(Action<ITaskContext> targetAction)
        {
            this.targetAction = targetAction;
            return this;
        }

        /// <summary>
        /// Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        public ITarget SetAsDefault()
        {
            targetTree.SetDefaultTarget(this);
            return this;
        }

        public ITarget SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Sets the target as hidden. Hidden targets will not be
        /// visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget"/>.</returns>
        public ITarget SetAsHidden()
        {
            isHidden = true;
            return this;
        }

        protected override bool LogDuration
        {
            get
            {
                return true;
            }
        }

        protected override void DoExecute(ITaskContext context)
        {
            targetTree.MarkTargetAsExecuted(this);
            targetTree.EnsureDependenciesExecuted(context, TargetName);

            // we can have action-less targets (that only depend on other targets)
            if (targetAction != null)
                targetAction(context);
        }

        protected override string DescriptionForLog
        {
            get
            {
                return targetName;
            }
        }

        private readonly List<string> dependencies = new List<string>();
        private string description;
        private bool isHidden;
        private readonly TargetTree targetTree;
        private readonly string targetName;
        private Action<ITaskContext> targetAction;
    }
}