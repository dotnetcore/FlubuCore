using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

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

        public TTargetFluentInterface Do(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(targetAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(targetAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(targetAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(targetAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(targetAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.Do(targetAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Func<ITaskContextInternal, T, T1, Task> targetAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContextInternal, T, T1, T2, Task> targetAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContextInternal, T, T1, T2, T3, Task> targetAction, T param, T1 param2, T2 param3, T3 param4,
            Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContextInternal, T, T1, T2, T3, T4, Task> targetAction, T param, T1 param2, T2 param3, T3 param4,
            T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null)
        {
            ActionCount = 1;
            LastTargetAction = TargetAction.AddTask;
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
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
    }
}