using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public class Target : TaskBase<int, Target>, ITargetInternal
    {
        private readonly Dictionary<string, TaskExecutionMode> _dependencies = new Dictionary<string, TaskExecutionMode>();

        private readonly List<TaskGroup> _taskGroups = new List<TaskGroup>();

        private readonly CommandArguments _args;

        private readonly TargetTree _targetTree;

        private Func<bool> _mustCondition;

        internal Target(TargetTree targetTree, string targetName, CommandArguments args)
        {
            if (targetName.Any(x => char.IsWhiteSpace(x)))
            {
                throw new ScriptException("Target name must not contain whitespaces.");
            }

            _targetTree = targetTree;
            TargetName = targetName;
            _args = args;
        }

        public Dictionary<string, TaskExecutionMode> Dependencies => _dependencies;

        public List<TaskGroup> TasksGroups => _taskGroups;

        /// <summary>
        ///     Gets a value indicating whether this target is hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; private set; }

        public string TargetName { get; }

        protected internal override string TaskName => TargetName;

        protected override bool LogDuration => true;

        protected override string Description { get; set; }

        string ITargetInternal.Description
        {
            get { return Description; }
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        public ITargetInternal DependsOn(params string[] targetNames)
        {
            foreach (string dependentTargetName in targetNames)
            {
                _dependencies.Add(dependentTargetName, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        public ITargetInternal DependsOnAsync(params string[] targetNames)
        {
            foreach (string dependentTargetName in targetNames)
            {
                _dependencies.Add(dependentTargetName, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITargetInternal DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask(targetAction);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);

            return this;
        }

        public ITargetInternal DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask2<T>(targetAction, param);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T, T2>(Action<ITaskContextInternal, T, T2> targetAction, T param, T2 param2, Action<DoTask3<T, T2>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T1, T2, T3>(Action<ITaskContextInternal, T1, T2, T3> targetAction, T1 param, T2 param2, T3 param3, Action<DoTask4<T1, T2, T3>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T1, T2, T3, T4>(Action<ITaskContextInternal, T1, T2, T3, T4> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTask5<T1, T2, T3, T4>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T1, T2, T3, T4, T5>(Action<ITaskContextInternal, T1, T2, T3, T4, T5> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTask6<T1, T2, T3, T4, T5>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync(targetAction);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync2<T>(targetAction, param);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T, T2>(Func<ITaskContextInternal, T, T2, Task> targetAction, T param, T2 param2, Action<DoTaskAsync3<T, T2>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T1, T2, T3>(Func<ITaskContextInternal, T1, T2, T3, Task> targetAction, T1 param, T2 param2, T3 param3, Action<DoTaskAsync4<T1, T2, T3>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T1, T2, T3, T4>(Func<ITaskContextInternal, T1, T2, T3, T4, Task> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTaskAsync5<T1, T2, T3, T4>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal DoAsync<T1, T2, T3, T4, T5>(Func<ITaskContextInternal, T1, T2, T3, T4, T5, Task> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTaskAsync6<T1, T2, T3, T4, T5>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITargetInternal Do(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask(targetAction);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITargetInternal Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask2<T>(targetAction, param);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITargetInternal Do<T, T2>(Action<ITaskContextInternal, T, T2> targetAction, T param, T2 param2, Action<DoTask3<T, T2>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITargetInternal Do<T1, T2, T3>(Action<ITaskContextInternal, T1, T2, T3> targetAction, T1 param, T2 param2, T3 param3, Action<DoTask4<T1, T2, T3>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITargetInternal Do<T1, T2, T3, T4>(Action<ITaskContextInternal, T1, T2, T3, T4> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTask5<T1, T2, T3, T4>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITargetInternal Do<T1, T2, T3, T4, T5>(Action<ITaskContextInternal, T1, T2, T3, T4, T5> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTask6<T1, T2, T3, T4, T5>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        public ITargetInternal SetAsDefault()
        {
            _targetTree.SetDefaultTarget(this);
            return this;
        }

        /// <summary>
        ///     Set's the description of the target,
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>this target</returns>
        public new ITargetInternal SetDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="ITargetInternal" />.</returns>
        public ITargetInternal SetAsHidden()
        {
            IsHidden = true;
            return this;
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency targets</param>
        /// <returns>This same instance of <see cref="ITargetInternal" /></returns>
        public ITargetInternal DependsOn(params ITargetInternal[] targets)
        {
            foreach (ITargetInternal target in targets)
            {
                _dependencies.Add(target.TargetName, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        public ITargetInternal DependsOnAsync(params ITargetInternal[] targets)
        {
            foreach (ITargetInternal target in targets)
            {
                _dependencies.Add(target.TargetName, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITargetInternal AddTask(TaskGroup taskGroup, params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        public ITargetInternal AddTaskAsync(TaskGroup taskGroup, params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITargetInternal Must(Func<bool> condition)
        {
            _mustCondition = condition;
            return this;
        }

        public void TargetHelp(ITaskContextInternal context)
        {
            _targetTree.MarkTargetAsExecuted(this);
            context.LogInfo(" ");
            context.LogInfo($"Target {TargetName} will execute next tasks:");

            for (int i = 0; i < _taskGroups.Count; i++)
            {
                for (int j = 0; j < _taskGroups[i].Tasks.Count; j++)
                {
                    var task = (TaskHelp)_taskGroups[i].Tasks[j].task;
                    task.LogTaskHelp(context);
                }
            }
        }

        public void RemoveLastAddedActionsFromTarget(TargetAction targetAction, int actionCount)
        {
            switch (targetAction)
            {
                case TargetAction.Other:
                    return;
                case TargetAction.AddTask:
                {
                    var lastGroup = TasksGroups.Last();
                    for (int i = 0; i < actionCount; i++)
                    {
                        var lastTask = lastGroup.Tasks.Last();
                        lastGroup.Tasks.Remove(lastTask);
                        if (lastGroup.Tasks.Count == 0)
                        {
                            TasksGroups.Remove(lastGroup);
                        }
                    }

                    return;
                }

                case TargetAction.AddDependency:
                {
                    for (int i = 0; i < actionCount; i++)
                    {
                        _dependencies.Remove(_dependencies.Keys.Last());
                    }

                    return;
                }
            }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_targetTree == null)
            {
                throw new ArgumentNullException(nameof(_targetTree), "TargetTree must be set before Execution of target.");
            }

            if (_mustCondition != null)
            {
                var conditionMeet = _mustCondition.Invoke();

                if (conditionMeet == false)
                {
                    throw new TaskExecutionException($"Condition in must was not meet. Failed to execute target: '{TargetName}'.", 50);
                }
            }

            context.LogInfo($"Executing target {TargetName}");

            _targetTree.EnsureDependenciesExecuted(context, TargetName);
            _targetTree.MarkTargetAsExecuted(this);

            if (_args.TargetsToExecute != null)
            {
                if (!_args.TargetsToExecute.Contains(TargetName))
                {
                    throw new TaskExecutionException($"Target {TargetName} is not on the TargetsToExecute list", 3);
                }
            }

            int taskGroupsCount = _taskGroups.Count;
            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            for (int i = 0; i < taskGroupsCount; i++)
            {
                int tasksCount = _taskGroups[i].Tasks.Count;
                if (_taskGroups[i].CleanupOnCancel)
                {
                    CleanUpStore.AddCleanupAction(_taskGroups[i].FinallyAction);
                }

                try
                {
                    for (int j = 0; j < tasksCount; j++)
                    {
                        var task = (TaskHelp)_taskGroups[i].Tasks[j].task;
                        context.LogInfo($"Executing task {task.TaskName}");
                        if (_taskGroups[i].Tasks[j].taskExecutionMode == TaskExecutionMode.Synchronous)
                        {
                            _taskGroups[i].Tasks[j].task.ExecuteVoid(context);
                        }
                        else
                        {
                            tasks.Add(_taskGroups[i].Tasks[j].task.ExecuteVoidAsync(context));
                            if (j + 1 < tasksCount)
                            {
                                if (_taskGroups[i].Tasks[j + 1].taskExecutionMode != TaskExecutionMode.Synchronous)
                                    continue;
                                if (tasks.Count <= 0) continue;
                                Task.WaitAll(tasks.ToArray());
                                tasks = new List<Task>();
                            }
                            else if (i + 1 < taskGroupsCount)
                            {
                                if (_taskGroups[i + 1].Tasks[0].taskExecutionMode != TaskExecutionMode.Synchronous)
                                    continue;
                                if (tasks.Count <= 0) continue;
                                Task.WaitAll(tasks.ToArray());
                                tasks = new List<Task>();
                            }
                            else if (tasksCount > 0)
                            {
                                Task.WaitAll(tasks.ToArray());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _taskGroups[i].OnErrorAction?.Invoke(context, e);
                    throw;
                }
                finally
                {
                    if (!CleanUpStore.StoreAccessed)
                    {
                        if (_taskGroups[i].CleanupOnCancel)
                        {
                            CleanUpStore.RemoveCleanupAction(_taskGroups[i].FinallyAction);
                        }

                        _taskGroups[i].FinallyAction?.Invoke(context);
                    }
                }
            }

            return 0;
        }

        private void AddTaskToTaskGroup(TaskGroup taskGroup, ITask task, TaskExecutionMode taskExecutionMode)
        {
            CheckThatTaskWasNotExecutedAlready(task);
            if (taskGroup == null)
            {
                taskGroup = new TaskGroup()
                {
                    GroupId = Guid.NewGuid().ToString(),
                };

                taskGroup.Tasks.Add((task, taskExecutionMode));
                TasksGroups.Add(taskGroup);
            }
            else
            {
                var existingGroup = TasksGroups.FirstOrDefault(x => x.GroupId == taskGroup.GroupId);
                if (existingGroup == null)
                {
                    taskGroup.Tasks.Add((task, taskExecutionMode));
                    TasksGroups.Add(taskGroup);
                }
                else
                {
                    taskGroup.Tasks.Add((task, taskExecutionMode));
                }
            }
        }

        private void CheckThatTaskWasNotExecutedAlready(ITask result)
        {
            if (result is TaskHelp taskBase)
            {
                if (taskBase.TaskExecuted)
                {
                    throw new ScriptException(
                        $"Calling Execute method on task in AddTask is not valid becasuse FlubuCore calls execute on AddTask implicitly and task would be executed every time build script is runned regardles which target is executed. Remove Execute method on task ${taskBase.TaskName}.");
                }
            }
        }
    }
}