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

        public ITargetFluentInterface Do(Action<ITaskContextInternal> targetAction)
        {
            Target.Do(targetAction);
            return this;
        }

        public ITargetFluentInterface DoAsync(Action<ITaskContextInternal> targetAction)
        {
            Target.DoAsync(targetAction);
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
