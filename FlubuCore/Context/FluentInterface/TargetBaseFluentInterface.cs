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

        public TTargetFluentInterface AddTask(params ITask[] tasks)
        {
            Target.AddTask(tasks);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task)
        {
            ITask result = task(TaskFluent);
            Target.AddTask(result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ITask result = task(CoreTaskFluent);
            Target.AddTask(result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task)
        {
            ITask result = task(TaskFluent);
            Target.AddTaskAsync(result);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ITask result = task(CoreTaskFluent);
            Target.AddTaskAsync(result);
            return this as TTargetFluentInterface;
        }

         public TTargetFluentInterface Do(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null)
        {
            Target.Do(targetAction, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            Target.Do(targetAction, param, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, param3, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, param3, param4, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            Target.Do(targetAction, param, param2, param3, param4, param5, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> doOptions = null)
        {
            Target.DoAsync(targetAction, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> doOptions = null)
        {
            Target.DoAsync(targetAction, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1>(Func<ITaskContextInternal, T, T1, Task> targetAction, T param, T1 param2, Action<DoTaskAsync3<T, T1>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2>(Func<ITaskContextInternal, T, T1, T2, Task> targetAction, T param, T1 param2, T2 param3, Action<DoTaskAsync4<T, T1, T2>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3>(Func<ITaskContextInternal, T, T1, T2, T3, Task> targetAction, T param, T1 param2, T2 param3, T3 param4,
            Action<DoTaskAsync5<T, T1, T2, T3>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, doOptions);
            return this as TTargetFluentInterface;
        }

        public TTargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Func<ITaskContextInternal, T, T1, T2, T3, T4, Task> targetAction, T param, T1 param2, T2 param3, T3 param4,
            T4 param5, Action<DoTaskAsync6<T, T1, T2, T3, T4>> doOptions = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, doOptions);
            return this as TTargetFluentInterface;
        }
    }
}
