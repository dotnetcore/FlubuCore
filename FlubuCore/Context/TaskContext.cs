using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContext : BuildPropertiesContext, ITaskContext
    {
        private readonly ITaskFactory _taskFactory;
        private readonly IFluentInterfaceFactory _fluentFactory;
        private readonly ILogger _log;

        public TaskContext(
            ILogger log,
            ITaskFactory taskFactory,
            IFluentInterfaceFactory fluentFactory,
            TargetTree targetTree,
            IBuildPropertiesSession properties)
            : base(properties)
        {
            _log = log;
            _taskFactory = taskFactory;
            _fluentFactory = fluentFactory;
            TargetTree = targetTree;
        }

        public TargetTree TargetTree { get; }

        public ITaskFluentInterface Tasks()
        {
            var t = _fluentFactory.GetTaskFluentInterface();
            t.Context = this;
            return t;
        }

        public ITargetFluentInterface CreateTarget(string name)
        {
            ITarget target = TargetTree.AddTarget(name);
            ITargetFluentInterface t = _fluentFactory.GetTargetFluentInterface();
            t.Target = target;
            TargetFluentInterface targetFluent = (TargetFluentInterface)t;
            targetFluent.Context = (TaskContextInternal)this;
            return targetFluent;
        }

        public ICoreTaskFluentInterface CoreTasks()
        {
            var t = _fluentFactory.GetCoreTaskFluentInterface();
            t.Context = this;
            return t;
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
