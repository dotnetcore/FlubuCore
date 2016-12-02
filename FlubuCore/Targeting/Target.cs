using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public class Target : TaskBase<int>, ITarget
    {
        private readonly List<string> _dependencies = new List<string>();

        private readonly List<ITask> _tasks = new List<ITask>();

        private readonly TargetTree _targetTree;

        private string _description;

        private Action<ITaskContextInternal> _targetAction;

        internal Target(TargetTree targetTree, string targetName)
        {
            _targetTree = targetTree;
            TargetName = targetName;
        }

        public ICollection<string> Dependencies => _dependencies;

        /// <summary>
        ///     Gets the description of the target.
        /// </summary>
        /// <value>The description of the target.</value>
        public string Description => _description;

        /// <summary>
        ///     Gets a value indicating whether this target is hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; private set; }

        public string TargetName { get; }

        protected override bool LogDuration => true;

        protected override string DescriptionForLog => TargetName;

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget DependsOn(params string[] targetNames)
        {
            foreach (var dependentTargetName in targetNames)
            {
                _dependencies.Add(dependentTargetName);
            }

            return this;
        }

        public ITarget Do(Action<ITaskContextInternal> targetAction)
        {
            if (_targetAction != null)
            {
                throw new ArgumentException("Target action was already set.");
            }

            _targetAction = targetAction;
            return this;
        }

        public ITarget OverrideDo(Action<ITaskContextInternal> targetAction)
        {
            _targetAction = targetAction;
            return this;
        }

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget SetAsDefault()
        {
            _targetTree.SetDefaultTarget(this);
            return this;
        }

        /// <summary>
        ///     Set's the description of the target,
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>this target</returns>
        public ITarget SetDescription(string description)
        {
            _description = description;
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
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency targets</param>
        /// <returns>This same instance of <see cref="ITarget" /></returns>
        public ITarget DependsOn(params ITarget[] targets)
        {
            foreach (var target in targets)
            {
                _dependencies.Add(target.TargetName);
            }

            return this;
        }

        public ITarget AddTask(params ITask[] tasks)
        {
            _tasks.AddRange(tasks);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_targetTree == null)
            {
                throw new ArgumentNullException(nameof(_targetTree), "TargetTree must be set before Execution of target.");
            }

            _targetTree.MarkTargetAsExecuted(this);
            _targetTree.EnsureDependenciesExecuted(context, TargetName);

            // we can have action-less targets (that only depend on other targets)
            _targetAction?.Invoke(context);

            foreach (var task in _tasks)
            {
                 task.ExecuteVoid(context);
            }

            return 0;
        }
    }
}