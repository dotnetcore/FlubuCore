using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Context.FluentInterface.TaskExtensions.Core;
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
            CoreTaskFluentInterface coreTask = (CoreTaskFluentInterface)t;
            coreTask.Context = (TaskContext)taskContext;
            LinuxTaskFluentInterface linuxTask = (LinuxTaskFluentInterface)coreTask.LinuxTasks();
            linuxTask.Context = (TaskContext)taskContext;
            return coreTask;
        }

        public ITaskFluentInterface GetTaskFluentInterface(ITaskContextInternal taskContext)
        {
            var t = _sp.GetRequiredService<ITaskFluentInterface>();
            TaskFluentInterface taskFluent = (TaskFluentInterface)t;
            taskFluent.Context = (TaskContext)taskContext;
            IisTaskFluentInterface iisTaskFluent = (IisTaskFluentInterface)taskFluent.IisTasks();
            iisTaskFluent.Context = (TaskContext)taskContext;
            return taskFluent;
        }

        public ITaskExtensionsFluentInterface GetTaskExtensionsFluentInterface(ITargetFluentInterface target, ITaskContextInternal taskContext)
        {
            var t = _sp.GetRequiredService<ITaskExtensionsFluentInterface>();
            TaskExtensionsFluentInterface taskExtensions = (TaskExtensionsFluentInterface)t;
            taskExtensions.Target = (TargetFluentInterface)target;
            taskExtensions.Context = taskContext;
            return taskExtensions;
        }

        public ICoreTaskExtensionsFluentInterface GetCoreTaskExtensionsFluentInterface(ITargetFluentInterface target, ITaskContextInternal taskContext)
        {
            var t = _sp.GetRequiredService<ICoreTaskExtensionsFluentInterface>();
            CoreTaskExtensionsFluentInterface coreTaskExtensions = (CoreTaskExtensionsFluentInterface)t;
            coreTaskExtensions.Target = (TargetFluentInterface)target;
            coreTaskExtensions.Context = taskContext;
            return coreTaskExtensions;
        }

        public ITargetFluentInterface GetTargetFluentInterface(ITarget target, ITaskContextInternal taskContext)
        {
            ITargetFluentInterface t = _sp.GetRequiredService<ITargetFluentInterface>();
            TargetFluentInterface targetFluent = (TargetFluentInterface)t;
            targetFluent.Target = target;
            targetFluent.Context = taskContext;
            targetFluent.CoreTaskFluent = GetCoreTaskFluentInterface(taskContext);
            targetFluent.TaskExtensionsFluent = GetTaskExtensionsFluentInterface(t, taskContext);
            targetFluent.CoreTaskExtensionsFluent = GetCoreTaskExtensionsFluentInterface(t, taskContext);
            targetFluent.TaskFluent = GetTaskFluentInterface(taskContext);

            return targetFluent;
        }
    }
}
