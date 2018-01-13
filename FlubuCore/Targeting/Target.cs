using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public class Target : TaskBase<int, Target>, ITarget
    {
        private readonly Dictionary<string, TaskExecutionMode> _dependencies = new Dictionary<string, TaskExecutionMode>();

        private readonly List<Tuple<ITask, TaskExecutionMode>> _tasks = new List<Tuple<ITask, TaskExecutionMode>>();

        private readonly CommandArguments _args;

        private readonly TargetTree _targetTree;

        internal Target(TargetTree targetTree, string targetName, CommandArguments args)
        {
            _targetTree = targetTree;
            TargetName = targetName;
            _args = args;
        }

        public Dictionary<string, TaskExecutionMode> Dependencies => _dependencies;

        public List<Tuple<ITask, TaskExecutionMode>> Tasks => _tasks;

        /// <summary>
        ///     Gets a value indicating whether this target is hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; private set; }

        public string TargetName { get; }

        protected override bool LogDuration => true;

        protected override string Description { get; set; }

        string ITarget.Description
        {
            get { return Description; }
        }

        protected override string DescriptionForLog => TargetName;

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget DependsOn(params string[] targetNames)
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
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget DependsOnAsync(params string[] targetNames)
        {
            foreach (string dependentTargetName in targetNames)
            {
                _dependencies.Add(dependentTargetName, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITarget DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null)
        {
            var task = new DoTask(targetAction);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null)
        {
            var task = new DoTask2<T>(targetAction, param);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T, T2>(Action<ITaskContextInternal, T, T2> targetAction, T param, T2 param2, Action<DoTask3<T, T2>> taskAction = null)
        {
            var task = new DoTask3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T1, T2, T3>(Action<ITaskContextInternal, T1, T2, T3> targetAction, T1 param, T2 param2, T3 param3, Action<DoTask4<T1, T2, T3>> taskAction = null)
        {
            var task = new DoTask4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4>(Action<ITaskContextInternal, T1, T2, T3, T4> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTask5<T1, T2, T3, T4>> taskAction = null)
        {
            var task = new DoTask5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4, T5>(Action<ITaskContextInternal, T1, T2, T3, T4, T5> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTask6<T1, T2, T3, T4, T5>> taskAction = null)
        {
            var task = new DoTask6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> taskAction = null)
        {
            var task = new DoTaskAsync(targetAction);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> taskAction = null)
        {
            var task = new DoTaskAsync2<T>(targetAction, param);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T, T2>(Func<ITaskContextInternal, T, T2, Task> targetAction, T param, T2 param2, Action<DoTaskAsync3<T, T2>> taskAction = null)
        {
            var task = new DoTaskAsync3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T1, T2, T3>(Func<ITaskContextInternal, T1, T2, T3, Task> targetAction, T1 param, T2 param2, T3 param3, Action<DoTaskAsync4<T1, T2, T3>> taskAction = null)
        {
            var task = new DoTaskAsync4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4>(Func<ITaskContextInternal, T1, T2, T3, T4, Task> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTaskAsync5<T1, T2, T3, T4>> taskAction = null)
        {
            var task = new DoTaskAsync5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4, T5>(Func<ITaskContextInternal, T1, T2, T3, T4, T5, Task> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTaskAsync6<T1, T2, T3, T4, T5>> taskAction = null)
        {
            var task = new DoTaskAsync6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            return this;
        }

        public ITarget Do(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null)
        {
            var task = new DoTask(targetAction);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            return this;
        }

        public ITarget Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null)
        {
            var task = new DoTask2<T>(targetAction, param);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            return this;
        }

        public ITarget Do<T, T2>(Action<ITaskContextInternal, T, T2> targetAction, T param, T2 param2, Action<DoTask3<T, T2>> taskAction = null)
        {
            var task = new DoTask3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            return this;
        }

        public ITarget Do<T1, T2, T3>(Action<ITaskContextInternal, T1, T2, T3> targetAction, T1 param, T2 param2, T3 param3, Action<DoTask4<T1, T2, T3>> taskAction = null)
        {
            var task = new DoTask4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            return this;
        }

        public ITarget Do<T1, T2, T3, T4>(Action<ITaskContextInternal, T1, T2, T3, T4> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTask5<T1, T2, T3, T4>> taskAction = null)
        {
            var task = new DoTask5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            return this;
        }

        public ITarget Do<T1, T2, T3, T4, T5>(Action<ITaskContextInternal, T1, T2, T3, T4, T5> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTask6<T1, T2, T3, T4, T5>> taskAction = null)
        {
            var task = new DoTask6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            Tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            return this;
        }

        public ITarget OverrideDo(Action<ITaskContextInternal> targetAction)
        {
            var tasks = Tasks.Select(x => x.Item1);
            var doTasks = tasks.OfType<DoTask>().ToList();
            if (doTasks.Count == 0)
            {
                return this;
            }

            if (doTasks.Count > 1)
            {
                throw new NotSupportedException("You can not override do when there are specified more than 1 do in target.");
            }

            doTasks[0] = new DoTask(targetAction);

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
        public new ITarget SetDescription(string description)
        {
            Description = description;
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
            foreach (ITarget target in targets)
            {
                _dependencies.Add(target.TargetName, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        public ITarget DependsOnAsync(params ITarget[] targets)
        {
            foreach (ITarget target in targets)
            {
                _dependencies.Add(target.TargetName, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITarget AddTask(params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                _tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Synchronous));
            }

            return this;
        }

        public ITarget AddTaskAsync(params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                _tasks.Add(new Tuple<ITask, TaskExecutionMode>(task, TaskExecutionMode.Parallel));
            }

            return this;
        }

        public void TargetHelp(ITaskContextInternal context)
        {
            _targetTree.MarkTargetAsExecuted(this);
            context.LogInfo(" ");
            context.LogInfo($"Target {TargetName} will execute next tasks:");

            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = (TaskHelp)_tasks[i].Item1;
                task.LogTaskHelp(context);
            }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_targetTree == null)
            {
                throw new ArgumentNullException(nameof(_targetTree), "TargetTree must be set before Execution of target.");
            }

            context.LogInfo($"Executing target {TargetName}");

            _targetTree.MarkTargetAsExecuted(this);
            _targetTree.EnsureDependenciesExecuted(context, TargetName);

            if (_args.TargetsToExecute != null)
            {
                if (!_args.TargetsToExecute.Contains(TargetName))
                {
                    throw new TaskExecutionException($"Target {TargetName} is not on the TargetsToExecute list", 3);
                }
            }

            int n = _tasks.Count;
            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            for (int i = 0; i < n; i++)
            {
                context.LogInfo($"Executing task {_tasks[i].Item1.GetType().Name}");
                if (_tasks[i].Item2 == TaskExecutionMode.Synchronous)
                {
                    _tasks[i].Item1.ExecuteVoid(context);
                }
                else
                {
                    var i1 = i;
                    tasks.Add(_tasks[i1].Item1.ExecuteVoidAsync(context));
                    if (i + 1 < n)
                    {
                        if (_tasks[i + 1].Item2 != TaskExecutionMode.Synchronous) continue;
                        if (tasks.Count <= 0) continue;
                        System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                        tasks = new List<System.Threading.Tasks.Task>();
                    }
                    else
                    {
                        if (tasks.Count > 0)
                        {
                            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                        }
                    }
                }
            }

            return 0;
        }
    }
}