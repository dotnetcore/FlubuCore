using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Targeting
{
    public class TargetTree
    {
        private readonly IServiceProvider _provider;
        private readonly CommandArguments _args;
        private readonly HashSet<string> _executedTargets = new HashSet<string>();

        private readonly Dictionary<string, ITarget> _targets = new Dictionary<string, ITarget>();

        public TargetTree(IServiceProvider provider, CommandArguments args)
        {
            _args = args;
            _provider = provider;

            AddTarget("help")
                .SetDescription("Displays the available targets in the build")
                .Do(TargetHelp);

            AddTarget("tasks")
                .SetDescription("Displays all registered tasks")
                .Do(TasksHelp);
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

        internal int DependenciesExecutedCount { get; private set; }

        public ITarget AddTarget(string targetName)
        {
            ITarget target = new Target(this, targetName, _args);
            _targets.Add(target.TargetName, target);
            return target;
        }

        public ITarget AddTarget(ITarget target)
        {
            _targets.Add(target.TargetName, target);
            return target;
        }

        public void EnsureDependenciesExecuted(ITaskContextInternal taskContext, string targetName)
        {
            ITarget target = _targets[targetName];
            int n = target.Dependencies.Count;
            List<Task> tTasks = new List<Task>();
            for (int i = 0; i < n; i++)
            {
                var dependantTargetName = target.Dependencies.Keys.ElementAt(i);
                var executionMode = target.Dependencies.Values.ElementAt(i);
                if (_executedTargets.Contains(dependantTargetName))
                    continue;

                if (_args.TargetsToExecute != null)
                {
                    if (!_args.TargetsToExecute.Contains(dependantTargetName))
                    {
                        throw new TaskExecutionException($"Target {dependantTargetName} is not on the TargetsToExecute list", 3);
                    }

                    DependenciesExecutedCount++;
                }
            
                if (executionMode == TaskExecutionMode.Synchronous)
                {
                    RunTarget(taskContext, dependantTargetName);
                }
                else
                {
                    tTasks.Add(Task.Run(() => RunTargetAsync(taskContext, targetName)));
                    if (i + 1 < n)
                    {
                        if (target.Dependencies.Values.ElementAt(i + 1) != TaskExecutionMode.Synchronous)
                            continue;
                        if (tTasks.Count <= 0)
                            continue;

                        Task.WaitAll(tTasks.ToArray());
                        tTasks = new List<Task>();
                    }
                    else
                    {
                        if (tTasks.Count > 0)
                        {
                            Task.WaitAll(tTasks.ToArray());
                        }
                    }
                }
            }
        }

        public IEnumerable<ITarget> EnumerateExecutedTargets()
        {
            foreach (var targetId in _executedTargets)
            {
                yield return _targets[targetId];
            }
        }

        public ITarget GetTarget(string targetName)
        {
            return _targets[targetName];
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
            return _targets.ContainsKey(targetName);
        }

        public void MarkTargetAsExecuted(ITarget target)
        {
            _executedTargets.Add(target.TargetName);
        }

        public void ResetTargetExecutionInfo()
        {
            _executedTargets.Clear();
        }

        public void RunTarget(ITaskContextInternal taskContext, string targetName)
        {
            if (!_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            ITarget target = _targets[targetName];
            target.ExecuteVoid(taskContext);
        }

        public async void RunTargetAsync(ITaskContextInternal taskContext, string targetName)
        {
            if (!_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            ITarget target = _targets[targetName];
            await target.ExecuteVoidAsync(taskContext);
        }

        public void SetDefaultTarget(ITarget target)
        {
            DefaultTarget = target;
        }

        /// <summary>
        ///     The target for displaying help in the command line.
        /// </summary>
        /// <param name="context">The task context.</param>
        public void TargetHelp(ITaskContextInternal context)
        {
            context.LogInfo("Targets:");

            // first sort the targets
            var sortedTargets = new SortedList<string, ITarget>();

            foreach (var target in _targets.Values)
            {
                sortedTargets.Add(target.TargetName, target);
            }

            // now display them in sorted order
            foreach (ITarget target in sortedTargets.Values)
            {
                if (target.IsHidden == false)
                {
                    context.LogInfo($"  {target.TargetName} : {target.Description}");
                }
            }
        }

        private void TasksHelp(ITaskContextInternal context)
        {
            context.LogInfo("Tasks:");

            // first sort the targets
            IEnumerable<ITask> tasks = _provider.GetServices<ITask>();

            // now display them in sorted order
            foreach (ITask task in tasks)
            {
                context.LogInfo($"  {task.GetType().FullName}");
            }
        }
    }
}