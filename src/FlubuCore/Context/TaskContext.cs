using System;
using System.Drawing;
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

        private readonly IBuildServer _buildServers;

        private readonly ILogger _log;

        public TaskContext(
            ILogger log,
            ITaskFactory taskFactory,
            IFluentInterfaceFactory fluentFactory,
            TargetTree targetTree,
            IBuildServer buildServers,
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

        protected int ExecutionDepth { get; set; }

        public ITaskFluentInterface Tasks()
        {
            return _fluentFactory.GetTaskFluentInterface((ITaskContextInternal)this);
        }

        public ITarget CreateTarget(string name)
        {
            ITargetInternal target = TargetTree.AddTarget(name);
            return _fluentFactory.GetTargetFluentInterface(target, (ITaskContextInternal)this);
        }

        public IBuildServer BuildServers()
        {
            return _buildServers;
        }

        public ICoreTaskFluentInterface CoreTasks()
        {
            return _fluentFactory.GetCoreTaskFluentInterface((ITaskContextInternal)this);
        }

        public virtual void LogInfo(string message)
        {
            FlubuConsoleLogger.Depth = ExecutionDepth;
            _log.LogInformation(message);
        }

        public virtual void LogInfo(string message, Color foregroundColor)
        {
            FlubuConsoleLogger.Color = foregroundColor;
            FlubuConsoleLogger.Depth = ExecutionDepth;
            _log.LogInformation(message);
        }

        public virtual void LogError(string message)
        {
            FlubuConsoleLogger.Depth = ExecutionDepth;
            _log.LogError(message);
        }

        public virtual void LogError(string message, Color foregroundColor)
        {
            FlubuConsoleLogger.Depth = ExecutionDepth;
            FlubuConsoleLogger.Color = foregroundColor;
            _log.LogError(message);
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
