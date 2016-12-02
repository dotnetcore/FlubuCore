using System;
using System.Linq;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface
{
    public class TargetFluentInterface : ITargetFluentInterface
    {
        private readonly ITaskFluentInterface _taskFluent;
        private readonly ICoreTaskFluentInterface _coreTaskFluent;
        private readonly ITaskExtensionsFluentInterface _taskExtensionsFluent;

        public TargetFluentInterface(ITaskFluentInterface taskFluent, ICoreTaskFluentInterface coreTaskFluent, ITaskExtensionsFluentInterface taskExtensionsFluent)
        {
            _taskFluent = taskFluent;
            _coreTaskFluent = coreTaskFluent;
            _taskExtensionsFluent = taskExtensionsFluent;
        }

        public ITarget Target { get; set; }

        public TaskContextInternal Context { protected get; set; }

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
            Target.DependsOn(targets.Select(i => i.Target).ToArray());
            return this;
        }

        public ITargetFluentInterface Do(Action<ITaskContextInternal> targetAction)
        {
            Target.Do(targetAction);
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
            var result = task(_taskFluent);
            Target.AddTask(result);
            return this;
        }

        public ITargetFluentInterface AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task)
        {
            var result = task(_coreTaskFluent);
            Target.AddTask(result);
            return this;
        }

        public ITaskExtensionsFluentInterface TaskExtensions()
        {
            var taskExtensionFluent = (TaskExtensionsFluentInterface)_taskExtensionsFluent;
            taskExtensionFluent.Target = this;
            taskExtensionFluent.Context = Context;
            return _taskExtensionsFluent;
        }

        public ITargetFluentInterface AddTask(params ITask[] tasks)
        {
            Target.AddTask(tasks);
            return this;
        }
    }
}
