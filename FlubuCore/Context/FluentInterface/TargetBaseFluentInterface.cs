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
        public ITarget Target { get; set; }

        public ITaskContextInternal Context { protected get; set; }

        internal ITaskFluentInterface TaskFluent { get; set; }

        internal ICoreTaskFluentInterface CoreTaskFluent { get; set; }

        internal ITaskExtensionsFluentInterface TaskExtensionsFluent { get; set; }

        internal ICoreTaskExtensionsFluentInterface CoreTaskExtensionsFluent { get; set; }

        protected TaskGroup TaskGroup { get; set; }

        public TTargetFluentInterface AddTask(params ITask[] tasks)
        {
            Target.AddTask(TaskGroup, tasks);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task)
        {
            ITask result = task(TaskFluent);
            Target.AddTask(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ITask result = task(CoreTaskFluent);
            Target.AddTask(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task)
        {
            ITask result = task(TaskFluent);
            Target.AddTaskAsync(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ITask result = task(CoreTaskFluent);
            Target.AddTaskAsync(TaskGroup, result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null)
        {
            Target.Do(targetAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            Target.Do(targetAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null)
        {
            Target.DoAsync(targetAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> doOptions = null)
        {
            Target.DoAsync(targetAction, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Func<ITaskContextInternal, T, T1, Task> targetAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContextInternal, T, T1, T2, Task> targetAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContextInternal, T, T1, T2, T3, Task> targetAction, T param, T1 param2, T2 param3, T3 param4,
            Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContextInternal, T, T1, T2, T3, T4, Task> targetAction, T param, T1 param2, T2 param3, T3 param4,
            T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, doOptions, TaskGroup);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface> action)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T> action, T param)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2> action, T param, T2 param2)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2, T3>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2, T3> action, T param, T2 param2, T3 param3)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2, param3);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2, T3, T4>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2, param3, param4);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2, T3, T4, T5>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2, param3, param4, param5);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2, T3, T4, T5, T6>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2, param3, param4, param5, param6);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2, T3, T4, T5, T6, T7>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6, T7 param7)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2, param3, param4, param5, param6, param7);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface When<T, T2, T3, T4, T5, T6, T7, T8>(Func<ITaskContextInternal, bool> condition, Action<TTargetFluentInterface, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6, T7 param7, T8 param8)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this as TTargetFluentInterface;
            }

            action?.Invoke(this as TTargetFluentInterface, param, param2, param3, param4, param5, param6, param7, param8);
            return this as TTargetFluentInterface;
        }
    }
}
