using System;
using System.Linq;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Context.FluentInterface.TaskExtensions.Core;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface
{
    public class TargetFluentInterface : ITargetFluentInterface
    {
        public ITarget Target { get; set; }

        public ITaskContextInternal Context { protected get; set; }

        internal ITaskFluentInterface TaskFluent { get; set; }

        internal ICoreTaskFluentInterface CoreTaskFluent { get; set; }

        internal ITaskExtensionsFluentInterface TaskExtensionsFluent { get; set; }

        internal ICoreTaskExtensionsFluentInterface CoreTaskExtensionsFluent { get; set; }

        public ITargetFluentInterface DependsOn(params string[] targetNames)
        {
            Target.DependsOn(targetNames);
            return this;
        }

        public ITargetFluentInterface DependsOn(params ITarget[] targets)
        {
            Target.DependsOn(targets);
            return this;
        }

        public ITargetFluentInterface DependsOn(params ITargetFluentInterface[] targets)
        {
            foreach (var t in targets)
            {
                var target = (TargetFluentInterface)t;
                Target.DependsOn(target.Target);
            }

            return this;
        }

        public ITargetFluentInterface DependsOnAsync(params ITarget[] targets)
        {
            Target.DependsOnAsync(targets);
            return this;
        }

        public ITargetFluentInterface DependsOnAsync(params ITargetFluentInterface[] targets)
        {
            foreach (var t in targets)
            {
                var target = (TargetFluentInterface)t;
                Target.DependsOnAsync(target.Target);
            }

            return this;
        }

        public ITargetFluentInterface Do(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null)
        {
            Target.Do(targetAction, taskAction);
            return this;
        }

        public ITargetFluentInterface Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null)
        {
            Target.Do(targetAction, param, taskAction);
            return this;
        }


        public ITargetFluentInterface Do<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> taskAction = null)
        {
            Target.Do(targetAction, param, param2, taskAction);
            return this;
        }


        public ITargetFluentInterface Do<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> taskAction = null)
        {
            Target.Do(targetAction, param, param2, param3, taskAction);
            return this;
        }


        public ITargetFluentInterface Do<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> taskAction = null)
        {
            Target.Do(targetAction, param, param2, param3, param4, taskAction);
            return this;
        }


        public ITargetFluentInterface Do<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> taskAction = null)
        {
            Target.Do(targetAction, param, param2, param3, param4, param5, taskAction);
            return this;
        }

        public ITargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null)
        {
            Target.DoAsync(targetAction, taskAction);
            return this;
        }

        public ITargetFluentInterface DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null)
        {
            Target.DoAsync(targetAction, param, taskAction);
            return this;
        }


        public ITargetFluentInterface DoAsync<T, T1>(Action<ITaskContextInternal, T, T1> targetAction, T param, T1 param2, Action<DoTask3<T, T1>> taskAction = null)
        {
            Target.DoAsync(targetAction, param, param2, taskAction);
            return this;
        }


        public ITargetFluentInterface DoAsync<T, T1, T2>(Action<ITaskContextInternal, T, T1, T2> targetAction, T param, T1 param2, T2 param3, Action<DoTask4<T, T1, T2>> taskAction = null)
        {
            Target.Do(targetAction, param, param2, param3, taskAction);
            return this;
        }


        public ITargetFluentInterface DoAsync<T, T1, T2, T3>(Action<ITaskContextInternal, T, T1, T2, T3> targetAction, T param, T1 param2, T2 param3, T3 param4, Action<DoTask5<T, T1, T2, T3>> taskAction = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, taskAction);
            return this;
        }


        public ITargetFluentInterface DoAsync<T, T1, T2, T3, T4>(Action<ITaskContextInternal, T, T1, T2, T3, T4> targetAction, T param, T1 param2, T2 param3, T3 param4, T4 param5, Action<DoTask6<T, T1, T2, T3, T4>> taskAction = null)
        {
            Target.DoAsync(targetAction, param, param2, param3, param4, param5, taskAction);
            return this;
        }


        public ITargetFluentInterface SetAsDefault()
        {
            Target.SetAsDefault();
            return this;
        }

        public ITargetFluentInterface SetDescription(string description)
        {
            Target.SetDescription(description);
            return this;
        }

        public ITargetFluentInterface SetAsHidden()
        {
            Target.SetAsHidden();
            return this;
        }

        public ITargetFluentInterface AddTask(Func<ITaskFluentInterface, ITask> task)
        {
            ITask result = task(TaskFluent);
            Target.AddTask(result);
            return this;
        }

        public ITargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ITask result = task(CoreTaskFluent);
            Target.AddTask(result);
            return this;
        }

        public ITargetFluentInterface AddTaskAsync(Func<ITaskFluentInterface, ITask> task)
        {
            ITask result = task(TaskFluent);
            Target.AddTaskAsync(result);
            return this;
        }

        public ITargetFluentInterface AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task)
        {
            ITask result = task(CoreTaskFluent);
            Target.AddTaskAsync(result);
            return this;
        }


        public ITaskExtensionsFluentInterface TaskExtensions()
        {
            TaskExtensionsFluentInterface taskExtensionFluent = (TaskExtensionsFluentInterface)TaskExtensionsFluent;
            taskExtensionFluent.Target = this;
            taskExtensionFluent.Context = Context;
            return TaskExtensionsFluent;
        }

        public ICoreTaskExtensionsFluentInterface CoreTaskExtensions()
        {
            CoreTaskExtensionsFluentInterface coreTaskExtensionsFluent = (CoreTaskExtensionsFluentInterface) CoreTaskExtensionsFluent;
            coreTaskExtensionsFluent.Target = this;
            coreTaskExtensionsFluent.Context = Context;
            return coreTaskExtensionsFluent;
        }

        public ITargetFluentInterface AddTask(params ITask[] tasks)
        {
            Target.AddTask(tasks);
            return this;
        }
    }
}
