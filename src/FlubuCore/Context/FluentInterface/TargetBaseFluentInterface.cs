using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Task = System.Threading.Tasks.Task;

namespace FlubuCore.Context.FluentInterface
{
    public abstract class TargetBaseFluentInterface<TTargetFluentInterface> : ITargetBaseFluentInterfaceOfT<TTargetFluentInterface>
        where TTargetFluentInterface : class, ITargetBaseFluentInterface
    {
        public TargetBaseFluentInterface()
        {
            LastTargetAction = TargetAction.Other;
        }

        public ITargetInternal Target { get; set; }

        public ITaskContextInternal Context { protected get; set; }

        internal ITaskFluentInterface TaskFluent { get; set; }

        internal ICoreTaskFluentInterface CoreTaskFluent { get; set; }

        protected TaskGroup TaskGroup { get; set; }

        protected TargetAction LastTargetAction { get; set; }

        protected int ActionCount { get; set; }

        public TTargetFluentInterface AddTask(params ITask[] tasks)
        {
            ActionCount = tasks.Length;
            LastTargetAction = TargetAction.AddTask;
            Target.AddTask(TaskGroup, tasks);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            ITask result = task(TaskFluent);
            Target.AddTask(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            ITask result = task(CoreTaskFluent);
            Target.AddTask(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            ITask result = task(TaskFluent);
            Target.AddTaskAsync(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            ITask result = task(CoreTaskFluent);
            Target.AddTaskAsync(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do(Action<ITaskContext> doAction, Action<DoTask> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(doAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T>(Action<ITaskContext, T> doAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(doAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1>(Action<ITaskContext, T, T1> doAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(doAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2>(Action<ITaskContext, T, T1, T2> doAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(doAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContext, T, T1, T2, T3> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(doAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContext, T, T1, T2, T3, T4> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(doAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Action<ITaskContext> doAction, Action<DoTask> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Func<ITaskContext, Task> doAction, Action<DoTaskAsync> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Action<ITaskContext, T> doAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Func<ITaskContext, T, Task> doAction, T param, Action<DoTaskAsync2<T>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Action<ITaskContext, T, T1> doAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Func<ITaskContext, T, T1, Task> doAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContext, T, T1, T2> doAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContext, T, T1, T2, Task> doAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContext, T, T1, T2, T3> doAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContext, T, T1, T2, T3, Task> doAction, T param, T1 param2, T2 param3, T3 param4,
            Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContext, T, T1, T2, T3, T4> doAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContext, T, T1, T2, T3, T4, Task> doAction, T param, T1 param2, T2 param3, T3 param4,
            T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(doAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTarget(ITarget target)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            var t = (TargetFluentInterface)target;
            Target.AddTask(TaskGroup, t.Target);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTargetAsync(ITarget target)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            var t = (TargetFluentInterface)target;
            Target.AddTaskAsync(TaskGroup, t.Target);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When(Func<ITaskContext, bool> condition)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            Target.RemoveLastAddedActionsFromTarget(LastTargetAction, ActionCount);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Must(Func<bool> condition)
        {
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Must(Func<ITaskContext, bool> condition, string message = null)
        {
            Target.Must(condition, message);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface ForEach<T>(IEnumerable<T> collection, Action<T, TTargetFluentInterface> action)
        {
            var target = this as TTargetFluentInterface;
            foreach (var element in collection)
            {
                action(element, target);
            }

            return target;
        }

        public TTargetFluentInterface Requires<T>(Expression<Func<T>> parameter)
        {
            Target.Requires(parameter);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface SequentialLogging(bool enable)
        {
            Target.SequentialLogging = enable;
            return this as TTargetFluentInterface;
        }
    }
}