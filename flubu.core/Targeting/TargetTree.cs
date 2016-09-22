using System;
using System.Collections.Generic;
using Flubu.Tasks;

namespace Flubu.Targeting
{
    public class TargetTree
    {
        private readonly HashSet<string> executedTargets = new HashSet<string>();
        private readonly Dictionary<string, ITarget> targets = new Dictionary<string, ITarget>();

        public TargetTree()
        {
            AddTarget("help")
                .SetDescription("Displays the available targets in the build")
                .Do(TargetHelp);
        }

        /// <summary>
        ///     Gets the default target for this runner.
        /// </summary>
        /// <remarks>
        ///     The default target is the one which will be executed if
        ///     the target is not specified in the command line.
        /// </remarks>
        /// <value>The default target.</value>
        public ITarget DefaultTarget { get; private set; }

        public ITarget AddTarget(string targetName)
        {
            ITarget target = new Target(this, targetName);
            targets.Add(target.TargetName, target);
            return target;
        }

        public ITarget AddTarget(ITarget target)
        {
            targets.Add(target.TargetName, target);
            return target;
        }

        public void EnsureDependenciesExecuted(ITaskContext taskContext, string targetName)
        {
            var target = targets[targetName];
            foreach (var dependency in target.Dependencies)
            {
                if (!executedTargets.Contains(dependency))
                {
                    RunTarget(taskContext, dependency);
                }
            }
        }

        public IEnumerable<ITarget> EnumerateExecutedTargets()
        {
            foreach (var targetId in executedTargets)
            {
                yield return targets[targetId];
            }
        }

        public ITarget GetTarget(string targetName)
        {
            return targets[targetName];
        }

        /// <summary>
        ///     Determines whether the specified target exists.
        /// </summary>
        /// <param name="targetName">Name of the target.</param>
        /// <returns>
        ///     <c>true</c> if the specified target exists; otherwise, <c>false</c>.
        /// </returns>
        public bool HasTarget(string targetName)
        {
            return targets.ContainsKey(targetName);
        }

        public void MarkTargetAsExecuted(ITarget target)
        {
            executedTargets.Add(target.TargetName);
        }

        public void ResetTargetExecutionInfo()
        {
            executedTargets.Clear();
        }

        public int RunTarget(ITaskContext taskContext, string targetName)
        {
            if (!targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            var target = targets[targetName];
            return target.Execute(taskContext);
        }

        public void SetDefaultTarget(ITarget target)
        {
            DefaultTarget = target;
        }

        /// <summary>
        ///     The target for displaying help in the command line.
        /// </summary>
        /// <param name="context">The task context.</param>
        public void TargetHelp(ITaskContext context)
        {
            context.WriteMessage("Targets:");

            // first sort the targets
            var sortedTargets = new SortedList<string, ITarget>();

            foreach (var target in targets.Values)
            {
                sortedTargets.Add(target.TargetName, target);
            }

            // now display them in sorted order
            foreach (var target in sortedTargets.Values)
            {
                if (target.IsHidden == false)
                {
                    context.WriteMessage($"  {target.TargetName} : {target.Description}");
                }
            }
        }
    }
}