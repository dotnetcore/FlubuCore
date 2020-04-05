using System;
using System.Collections.Generic;
#if !NETSTANDARD1_6
using System.Drawing;
#endif
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

        private readonly Dictionary<string, ITargetInternal> _targets = new Dictionary<string, ITargetInternal>(StringComparer.OrdinalIgnoreCase);

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

        public List<(string actioName, TargetAction targetAction, string targetName)> BuildSummaryExtras { get; set; } = new List<(string actioName, TargetAction targetAction, string targetName)>();

        internal int DependenciesExecutedCount { get; private set; }

        internal IBuildScript BuildScript { get; set; }

        public virtual ITargetInternal AddTarget(string targetName)
        {
            if (_targets.ContainsKey(targetName))
            {
                throw new ArgumentException($"Target with the name '{targetName}' already exists");
            }

            ITargetInternal target = new Target(this, targetName, _args);
            _targets.Add(target.TargetName, target);
            return target;
        }

        public virtual ITargetInternal AddTarget(ITargetInternal target)
        {
            if (_targets.ContainsKey(target.TargetName))
            {
                throw new ArgumentException($"Target with the name '{target.TargetName}' already exists");
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
                var dependentTargetName = target.Dependencies.Keys.ElementAt(i);
                var executionMode = target.Dependencies.Values.ElementAt(i);
                if (_executedTargets.Contains(dependentTargetName))
                    continue;

                if (_args.TargetsToExecute != null)
                {
                    if (!_args.TargetsToExecute.Contains(dependentTargetName))
                    {
                        throw new TaskExecutionException($"Target {dependentTargetName} is not on the TargetsToExecute list", 3);
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
                        if (target.Dependencies.Values.ElementAt(i + 1) != TaskExecutionMode.Sync)
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
            _executedTargets.Clear();
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

        public virtual async Task RunTargetAsync(ITaskContextInternal taskContext, string targetName, bool sequentialLogging = false)
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

        public virtual void LogBuildSummary(IFlubuSession session)
        {
            int maxTargetNameLength = GetTargetNameMaxLength();

            LogTargetSummaryTitle();

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

                LogTargetSummary(targt);
            }

            if (session.Args.DryRun)
            {
                session.LogInfo("DRY RUN PERFORMED");
            }
            else if (session.UnknownTarget.HasValue && !session.UnknownTarget.Value)
            {
                TimeSpan buildDuration = session.BuildStopwatch.Elapsed;
                session.LogInfo(" ");

#if !NETSTANDARD1_6
                session.LogInfo(session.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL", session.HasFailed ? Color.Red : Color.Green);
                session.LogInfo($"Build finish time: {DateTime.Now:g}", Color.DimGray);
                session.LogInfo($"Build duration: {buildDuration.Hours:D2}:{buildDuration.Minutes:D2}:{buildDuration.Seconds:D2} ({(int)buildDuration.TotalSeconds:d} seconds)", Color.DimGray);
#else
                session.LogInfo(session.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL");
                session.LogInfo($"Build finish time: {DateTime.Now:g}");
                session.LogInfo(string.Format(
                    "Build duration: {0:D2}:{1:D2}:{2:D2} ({3:d} seconds)",
                    buildDuration.Hours,
                    buildDuration.Minutes,
                    buildDuration.Seconds,
                    (int)buildDuration.TotalSeconds));
#endif
            }

            const string TargetTitle = "Target";
            const string DurationTitle = "Duration";
            const string StatusTitle = "Status";
            const int DurationLength = 8;
            const int StatusLength = 10;

            int GetTargetNameMaxLength()
            {
                var executedTargetNames = _targets.Where(t => _executedTargets.Contains(t.Key))
                                                  .Select(t => t.Value.TargetName);
                int maxLength = executedTargetNames.Max(s => s.Length);
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

#if !NETSTANDARD1_6
            Color GetStatusColor(TaskStatus status)
            {
                var color = Color.Black;
                switch (status)
                {
                    case TaskStatus.Failed:
                        color = Color.Red;
                        break;
                    case TaskStatus.Finished:
                        color = Color.Green;
                        break;
                    default:
                        break;
                }

                return color;
            }
#endif
            void LogTargetSummary(Target t)
            {
                var targetName = t.TargetName.Capitalize().PadRight(maxTargetNameLength + 2);
                var duration = t.TaskStopwatch.Elapsed.ToString(@"hh\:mm\:ss").PadRight(DurationLength + 2);
                var status = t.TaskStatus.ToString().PadRight(StatusLength + 2);
#if !NETSTANDARD1_6
                var color = GetStatusColor(t.TaskStatus);
                session.LogInfo($"{targetName}{duration}{status}", color);
#else
                session.LogInfo($"{targetName}{duration}{status}");
#endif
            }
        }

        protected virtual void AddDefaultTargets()
        {
            AddTarget("help")
                .SetDescription("Displays the available targets in the build")
                .SetLogDuration(false)
                .Do(LogTargetsWithHelp);

            AddTarget("help.onlyTargets")
                .SetDescription("Displays the available targets in the build")
                .SetLogDuration(false)
                .SetLogExecutionInfo(false)
                .SetAsHidden()
                .Do(LogTargetsHelp);

            AddTarget("tasks")
                .SetDescription("Displays all registered tasks")
                .SetLogDuration(false)
                .Do(LogTasksHelp);
        }
    }
}