using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Context.FluentInterface
{
    public class FluentInterfaceFactory : IFluentInterfaceFactory
    {
        private readonly IServiceProvider _sp;

        public FluentInterfaceFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public ICoreTaskFluentInterface GetCoreTaskFluentInterface(ITaskContextInternal taskContext)
        {
            var t = _sp.GetRequiredService<ICoreTaskFluentInterface>();
            t.Context = (TaskContext)taskContext;
            t.LinuxTasks().Context = (TaskContext)taskContext;
            return t;
        }

        public ITaskFluentInterface GetTaskFluentInterface(ITaskContextInternal taskContext)
        {
            var t = _sp.GetRequiredService<ITaskFluentInterface>();
            t.Context = (TaskContext)taskContext;
            t.IisTasks().Context = (TaskContext)taskContext;
            return t;
        }

        public ITaskExtensionsFluentInterface GetTaskExtensionsFluentInterface(ITargetFluentInterface target, ITaskContextInternal taskContext)
        {
            var t = _sp.GetRequiredService<ITaskExtensionsFluentInterface>();
            t.Target = target;
            t.Context = taskContext;
            return t;
        }

        public ITargetFluentInterface GetTargetFluentInterface(ITarget target, ITaskContextInternal taskContext)
        {
            ITargetFluentInterface t = _sp.GetRequiredService<ITargetFluentInterface>();
            t.Target = target;
            t.Context = taskContext;

            TargetFluentInterface targetFluent = (TargetFluentInterface)t;
            targetFluent.CoreTaskFluent = GetCoreTaskFluentInterface(taskContext);
            targetFluent.TaskExtensionsFluent = GetTaskExtensionsFluentInterface(t, taskContext);
            targetFluent.TaskFluent = GetTaskFluentInterface(taskContext);

            return t;
        }
    }
}
