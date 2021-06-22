using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Targeting
{
    public partial class TargetTree
    {
        private readonly IServiceProvider _provider;

        private readonly CommandArguments _args;

        private readonly HashSet<string> _executedTargets = new HashSet<string>();

        private readonly Dictionary<string, ITargetInternal> _targets =
            new Dictionary<string, ITargetInternal>(StringComparer.OrdinalIgnoreCase);

        public TargetTree(IServiceProvider provider, CommandArguments args)
        {
            _args = args;
            _provider = provider;
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

        public List<(string actioName, TargetAction targetAction, string targetName)> BuildSummaryExtras { get; set; } =
            new List<(string actioName, TargetAction targetAction, string targetName)>();

        internal int DependenciesExecutedCount { get; private set; }

        internal IBuildScript BuildScript { get; set; }

        public virtual ITargetInternal AddTarget(string targetName)
        {
            if (_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"Target with the name '{targetName}' already exists. Rename duplicated target.");
            }

            ITargetInternal target = new Target(this, targetName, _args);
            _targets.Add(target.TargetName, target);
            return target;
        }

        public virtual ITargetInternal AddTarget(ITargetInternal target)
        {
            if (_targets.ContainsKey(target.TargetName))
            {
                throw new ArgumentException($"Target with the name '{target.TargetName}' already exists. Rename duplicated target.");
            }

            _targets.Add(target.TargetName, target);
            return target;
        }

        public virtual void EnsureDependenciesExecuted(ITaskContextInternal taskContext, string targetName)
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
                var dependentTarget = target.Dependencies[i];
                if (dependentTarget.Skipped)
                {
                    continue;
                }

                var dependentTargetName = dependentTarget.TargetName;
                var executionMode = dependentTarget.TaskExecutionMode;

                if (_executedTargets.Contains(dependentTargetName))
                    continue;

                if (_args.TargetsToExecute != null)
                {
                    if (!_args.TargetsToExecute.Contains(dependentTargetName))
                    {
                        throw new TaskExecutionException(
                            $"Target {dependentTargetName} is not on the TargetsToExecute list", 3);
                    }

                    DependenciesExecutedCount++;
                }

                if (executionMode == TaskExecutionMode.Sync)
                {
                    RunTarget(taskContext, dependentTargetName);
                }
                else
                {
                    tasks.Add(RunTargetAsync(taskContext, dependentTargetName, target.SequentialLogging));
                    if (i + 1 < n)
                    {
                        if (target.Dependencies[i + 1].TaskExecutionMode != TaskExecutionMode.Sync)
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

        public virtual void ResetTargetTree()
        {
            _targets.Clear();
            ResetTargetExecutionInfo();
            AddDefaultTargets();
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

        public IEnumerable<string> GetTargetNames()
        {
            return _targets.Select(x => x.Value.TargetName);
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

        public virtual void MarkTargetAsExecuted(ITargetInternal target)
        {
            _executedTargets.Add(target.TargetName);
        }

        public void ResetTargetExecutionInfo()
        {
            _executedTargets.Clear();
        }

        public virtual void RunTarget(ITaskContextInternal taskContext, string targetName)
        {
            if (!_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            ITargetInternal target = _targets[targetName];
            target.ExecuteVoid(taskContext);
        }

        public virtual async Task RunTargetAsync(ITaskContextInternal taskContext, string targetName,
            bool sequentialLogging = false)
        {
            if (!_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"The target '{targetName}' does not exist");
            }

            ITargetInternal target = _targets[targetName];
            target.SequentialLogging = sequentialLogging;
            await target.ExecuteVoidAsync(taskContext);
        }

        public virtual void RunTargetHelp(ITaskContextInternal taskContext, string targetName)
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
                taskContext.LogInfo($"Target {targetName.Capitalize()} dependencies: ");
                foreach (var dependent in target.Dependencies)
                {
                    var targetDependecy = _targets[dependent.TargetName] as Target;
                    targetDependecy?.TargetHelp(taskContext);
                }
            }
        }

        public void SetDefaultTarget(ITargetInternal target)
        {
            DefaultTargets.Add(target);
        }

        public virtual void LogBuildSummary(IFlubuSession session)
        {
            foreach (var target in EnumerateExecutedTargets())
            {
                var targt = target as Target;

                if (targt?.TaskStopwatch.ElapsedTicks > 0)
                {
                    foreach (var buildSummaryExtra in BuildSummaryExtras)
                    {
                        if (buildSummaryExtra.targetName != target.TargetName)
                        {
                            continue;
                        }

                        switch (buildSummaryExtra.targetAction)
                        {
                            case TargetAction.AddDependency:
                            {
                                session.LogInfo($"  Target dependence '{buildSummaryExtra.actioName}' was skipped.");
                                break;
                            }

                            case TargetAction.AddTask:
                            {
                                session.LogInfo($"  Target task '{buildSummaryExtra.actioName}' was skipped.");
                                break;
                            }
                        }
                    }
                }
            }

            if (session.Args.DryRun)
            {
                session.LogInfo("DRY RUN PERFORMED");
            }
            else if (session.UnknownTarget.HasValue && !session.UnknownTarget.Value)
            {
                LogBuildSummaryTable(session);
            }
        }

        public virtual void LogBuildSummaryTable(IFlubuSession session)
        {
            const string TargetTitle = "Target";
            const string DurationTitle = "Duration";
            const string StatusTitle = "Status";
            const int DurationLength = 8;
            const int StatusLength = 10;

            var targetsInOrder = GetTargetsInExecutionOrder(session);
            if (targetsInOrder.Count < 1 || targetsInOrder[0].TargetName
                .Equals(FlubuTargets.Help, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            int maxTargetNameLength = GetTargetNameMaxLength(targetsInOrder);
            LogTargetSummaryTitle();

            foreach (var target in targetsInOrder)
            {
                if (!target.IsHidden)
                {
                    LogTargetSummary(target as Target);
                }
            }

            TimeSpan buildDuration = session.BuildStopwatch.Elapsed;
            session.LogInfo(" ");
            session.LogInfo(session.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL", session.HasFailed ? Color.Red : Color.Green);
            session.LogInfo($"Build finish time: {DateTime.Now:g}", Color.DimGray);
            session.LogInfo(
                $"Build duration: {buildDuration.Hours:D2}:{buildDuration.Minutes:D2}:{buildDuration.Seconds:D2} ({(int)buildDuration.TotalSeconds:d} seconds)", Color.DimGray);

            int GetTargetNameMaxLength(List<ITargetInternal> targets)
            {
                if (targets.Count < 1)
                {
                    throw new ArgumentException($"Argument {nameof(targets)} should have at least one element.");
                }

                var targetNames = targets.Select(t => t.TargetName);
                int maxLength = targetNames.Max(s => s.Length);
                if (maxLength < TargetTitle.Length)
                {
                    maxLength = TargetTitle.Length;
                }

                return maxLength;
            }

            void LogTargetSummaryTitle()
            {
                session.LogInfo(Environment.NewLine + Environment.NewLine +
                                TargetTitle.PadRight(maxTargetNameLength + 2) +
                                DurationTitle.PadRight(DurationLength + 2) +
                                StatusTitle);
                session.LogInfo(new string('-', maxTargetNameLength + 2) +
                                new string('-', DurationLength + 2) +
                                new string('-', StatusLength + 2));
            }

            Color GetStatusColor(TaskStatus status)
            {
                var color = Color.DimGray;
                switch (status)
                {
                    case TaskStatus.Failed:
                        color = Color.Red;
                        break;
                    case TaskStatus.Succeeded:
                        color = Color.Green;
                        break;
                    default:
                        break;
                }

                return color;
            }

            void LogTargetSummary(Target t)
            {
                var targetName = t.TargetName.Capitalize().PadRight(maxTargetNameLength + 2);
                var duration = t.TaskStopwatch.Elapsed.ToString(@"hh\:mm\:ss").PadRight(DurationLength + 2);
                var status = t.TaskStatus.ToString().PadRight(StatusLength + 2);
                var color = GetStatusColor(t.TaskStatus);
                session.LogInfo($"{targetName}{duration}{status}", color);
            }
        }

        public List<ITargetInternal> GetTargetsInExecutionOrder(IFlubuSession session, bool includeTargetsWithoutTasks = true)
        {
            IEnumerable<string> targetNames = session.Args.MainCommands;
            if (targetNames == null || !targetNames.Any())
            {
                var targetsInOrder = new List<ITargetInternal>();
                if (_executedTargets?.Count < 1 && DefaultTargets.Count < 1)
                {
                    return targetsInOrder;
                }

                targetNames = DefaultTargets.Select(t => t.TargetName);
            }

            return GetTargetsInExecutionOrder(targetNames, includeTargetsWithoutTasks);
        }

        public List<ITargetInternal> GetTargetsInExecutionOrder(IEnumerable<string> targetNames, bool includeTargetsWithoutTasks = true)
        {
            var targetsInOrder = new List<ITargetInternal>();

            foreach (var targetName in targetNames)
            {
                AddDependentTargets(_targets[targetName]);
            }

            return targetsInOrder;

            void AddDependentTargets(ITargetInternal target)
            {
                foreach (var dependent in target.Dependencies)
                {
                    var nextTarget = _targets[dependent.TargetName];
                    AddDependentTargets(nextTarget);
                }

                if (!targetsInOrder.Exists(t => t.TargetName.Equals(target.TargetName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!includeTargetsWithoutTasks)
                    {
                        if (!target.TasksGroups.Any())
                        {
                            return;
                        }
                    }

                    targetsInOrder.Add(target);
                }
            }
        }

        protected virtual void AddDefaultTargets()
        {
            AddTarget(FlubuTargets.Help)
                .SetDescription("Displays the available targets in the build")
                .SetLogDuration(false)
                .Do(LogTargetsWithHelp);

            AddTarget(FlubuTargets.HelpOnlyTargets)
                .SetDescription("Displays the available targets in the build")
                .SetLogDuration(false)
                .SetLogExecutionInfo(false)
                .SetAsHidden()
                .Do(LogTargetsHelp);

            AddTarget("tasks")
                .SetDescription("Displays all registered tasks")
                .SetAsHidden()
                .SetLogDuration(false)
                .Do(LogTasksHelp);
        }
    }
}