using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContext : BuildPropertiesContext, ITaskContext
    {
        private readonly ITaskFluentInterface _taskFluentInterface;

        private readonly ICoreTaskFluentInterface _coreTaskFluentInterface;

        private readonly ITaskFactory _taskFactory;

        private readonly ITargetFluentInterface _targetFluent;

        private readonly ILogger _log;

        public TaskContext(
            ILogger log,
            ITaskFactory taskFactory,
            ICoreTaskFluentInterface coreTaskFluentInterface,
            ITaskFluentInterface taskFluentInterface,
            ITargetFluentInterface targetFluent,
            TargetTree targetTree,
            IBuildPropertiesSession properties)
            : base(properties)
        {
            _log = log;
            _taskFactory = taskFactory;
            _taskFluentInterface = taskFluentInterface;
            _coreTaskFluentInterface = coreTaskFluentInterface;
            TargetTree = targetTree;
            _targetFluent = targetFluent;
            _taskFluentInterface.Context = this;
            _coreTaskFluentInterface.Context = this;
        }

        public TargetTree TargetTree { get; }

        public ITaskFluentInterface Tasks()
        {
            return _taskFluentInterface;
        }

        public ITargetFluentInterface CreateTarget(string name)
        {
            var target = TargetTree.AddTarget(name);
            _targetFluent.Target = target;
            var targetFluent = (TargetFluentInterface)_targetFluent;
            targetFluent.Context = (TaskContextInternal)this;
            return targetFluent;
        }

        public ICoreTaskFluentInterface CoreTasks()
        {
            return _coreTaskFluentInterface;
        }

        public void LogInfo(string message)
        {
            _log?.LogInformation(message);
        }

        public void LogError(string message)
        {
            _log?.LogError(message);
        }

        internal T CreateTask<T>()
        where T : ITask
        {
            return _taskFactory.Create<T>();
        }

        internal T CreateTask<T>(params object[] constructorArgs)
            where T : ITask
        {
            return _taskFactory.Create<T>(constructorArgs);
        }
    }
}
