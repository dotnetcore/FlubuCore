using System;
#if  !NETSTANDARD1_6
using System.Drawing;
#endif
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContext : BuildPropertiesContext, ITaskContext
    {
        private readonly ITaskFactory _taskFactory;
        private readonly IFluentInterfaceFactory _fluentFactory;
        private readonly IBuildSystem _buildServers;
        private readonly ILogger _log;

        public TaskContext(
            ILogger log,
            ITaskFactory taskFactory,
            IFluentInterfaceFactory fluentFactory,
            TargetTree targetTree,
            IBuildSystem buildServers,
            IBuildPropertiesSession properties)
            : base(properties)
        {
            _log = log;
            _taskFactory = taskFactory;
            _fluentFactory = fluentFactory;
            _buildServers = buildServers;
            TargetTree = targetTree;
        }

        public TargetTree TargetTree { get; }

        public ITaskFluentInterface Tasks()
        {
            return _fluentFactory.GetTaskFluentInterface((ITaskContextInternal)this);
        }

        public ITarget CreateTarget(string name)
        {
            ITargetInternal target = TargetTree.AddTarget(name);
            return _fluentFactory.GetTargetFluentInterface(target, (ITaskContextInternal)this);
        }

        public IBuildSystem BuildSystems()
        {
            return _buildServers;
        }

        public ICoreTaskFluentInterface CoreTasks()
        {
            return _fluentFactory.GetCoreTaskFluentInterface((ITaskContextInternal)this);
        }

        public void LogInfo(string message)
        {
            _log.LogInformation(message);
        }

        #if !NETSTANDARD1_6
        public void LogInfo(string message, Color foregroundColor)
        {
            FlubuConsoleLogger.Color = foregroundColor;
            _log.LogInformation(message);
        }
        #endif

        public void LogError(string message)
        {
            _log.LogError(message);
        }

#if !NETSTANDARD1_6
        public void LogError(string message, Color foregroundColor)
        {
            FlubuConsoleLogger.Color = foregroundColor;
            _log.LogError(message);
        }
#endif

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
