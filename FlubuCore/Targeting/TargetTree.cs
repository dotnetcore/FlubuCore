using System;
using System.Collections.Generic;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Targeting
{
    public class TargetTree
    {
        private readonly IServiceProvider _provider;
        private readonly CommandArguments _args;
        private readonly HashSet<string> _executedTargets = new HashSet<string>();
        private readonly Dictionary<string, ITargetInternal> _targets = new Dictionary<string, ITargetInternal>(StringComparer.OrdinalIgnoreCase);

        public TargetTree(IServiceProvider provider, CommandArguments args)
        {
            _args = args;
            _provider = provider;

            AddTarget("help")
                .SetDescription("Displays the available targets in the build")
                .Do(TargetsHelp);

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
        public List<ITargetInternal> DefaultTargets { get; private set; } = new List<ITargetInternal>();

        public int TargetCount => _targets.Count;

        public List<string> ScriptArgsHelp { get; set; }

        internal int DependenciesExecutedCount { get; private set; }

        public ITargetInternal AddTarget(string targetName)
        {
            if (_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"Target with the name '{targetName}' already exists");
            }

            ITargetInternal target = new Target(this, targetName, _args);
            _targets.Add(target.TargetName, target);
            return target;
        }

        public ITargetInternal AddTarget(ITargetInternal target)
        {
            if (_targets.ContainsKey(target.TargetName))
            {
                throw new ArgumentException($"Target with the name '{target.TargetName}' already exists");
            }

            _targets.Add(target.TargetName, target);
            return target;
        }

        public void EnsureDependenciesExecuted(ITaskContextInternal taskContext, string targetName)
        {
            if (_args.NoDependencies)
            {
                taskContext.LogInfo("Skipping target dependencies.");
                return;
            }

            ITargetInternal target = _targets[targetName];
            int n = target.Dependencies.Count;
            List<Task> tasks = new List<Task>();
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
                    tasks.Add(RunTargetAsync(taskContext, dependantTargetName));
                    if (i + 1 < n)
                    {
                        if (target.Dependencies.Values.ElementAt(i + 1) != TaskExecutionMode.Synchronous)
                            continue;
                        if (tasks.Count <= 0)
                            continue;

                        Task.WaitAll(tasks.ToArray());
                        tasks = new List<Task>();
                    }
                    else
                    {
                        if (tasks.Count > 0)
                        {
                            Task.WaitAll(tasks.ToArray());
                        }
                    }
                }
            }
        }

        public IEnumerable<ITargetInternal> EnumerateExecutedTargets()
        {
            foreach (var targetId in _executedTargets)
            {
                yield return _targets[targetId];
            }
        }

        public ITargetInternal GetTarget(string targetName)
        {
            return _targets[targetName];
        }

        /// <summary>
        ///     Determines whether the specified targets exists.
        /// </summary>
        /// <param name="targetNames">Name of the target.</param>
        /// <returns>
        ///     <c>true</c> if the specified target exists; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAllTargets(List<string> targetNames, out List<string> notFoundTargets)
        {
            notFoundTargets = new List<string>();
            foreach (var targetName in targetNames)
            {
                if (!HasTarget(targetName))
                {
                    notFoundTargets.Add(targetName);
                }
            }

            return notFoundTargets.Count == 0;
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

        public void MarkTargetAsExecuted(ITargetInternal target)
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

            ITargetInternal target = _targets[targetName];
            target.ExecuteVoid(taskContext);
        }

        public async Task RunTargetAsync(ITaskContextInternal taskContext, string targetName)
        {
            if (!_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            ITargetInternal target = _targets[targetName];
            await target.ExecuteVoidAsync(taskContext);
        }

        public void RunTargetHelp(ITaskContextInternal taskContext, string targetName)
        {
            if (!_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            Target target = _targets[targetName] as Target;

            target.TargetHelp(taskContext);

            if (target.Dependencies.Count > 0)
            {
                taskContext.LogInfo(" ");
                taskContext.LogInfo($"Target {targetName}  dependencies: ");
                foreach (var targetDependencyName in target.Dependencies)
                {
                    var targetDependecy = _targets[targetDependencyName.Key] as Target;
                    targetDependecy?.TargetHelp(taskContext);
                }
            }
        }

        public void SetDefaultTarget(ITargetInternal target)
        {
            DefaultTargets.Add(target);
        }

        /// <summary>
        ///     The target for displaying help in the command line.
        /// </summary>
        /// <param name="context">The task context.</param>
        public void TargetsHelp(ITaskContextInternal context)
        {
            if (context != null && !string.IsNullOrEmpty(context.FlubuHelpText))
            {
                context.LogInfo(context.FlubuHelpText);
                context.LogInfo(string.Empty);
            }

            context.LogInfo("Targets:");

            // first sort the targets
            var sortedTargets = new SortedList<string, ITargetInternal>();

            foreach (var target in _targets.Values)
            {
                sortedTargets.Add(target.TargetName, target);
            }

            // now display them in sorted order
            foreach (ITargetInternal target in sortedTargets.Values)
            {
                if (target.IsHidden == false)
                {
                    context.LogInfo($"  {target.TargetName} : {target.Description}");
                }
            }

            if (ScriptArgsHelp?.Count > 0)
            {
                context.LogInfo(" ");
                context.LogInfo("Global build script arguments:");
                foreach (var argHelp in ScriptArgsHelp)
                {
                    context.LogInfo($"  {argHelp}");
                }

                context.LogInfo(" ");
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