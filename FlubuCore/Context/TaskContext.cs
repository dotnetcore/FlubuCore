using System;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContext : ITaskContext
    {
        private readonly ILogger _log;

        private readonly ITaskFactory _taskFactory;

        private readonly ITaskFluentInterface _taskFluentInterface;

        private readonly ICoreTaskFluentInterface _coreTaskFluentInterface;

        private bool _disposed;

        private int _executionDepth;

        public TaskContext(
            ILogger log,
            IBuildPropertiesSession taskContextProperties,
            CommandArguments args,
            ITaskFactory taskFactory,
            ICoreTaskFluentInterface coreTaskFluentInterface,
            ITaskFluentInterface taskFluentInterface)
        {
            _log = log;
            _taskFactory = taskFactory;
            Args = args;
            Properties = taskContextProperties;
            _coreTaskFluentInterface = coreTaskFluentInterface;
            _taskFluentInterface = taskFluentInterface;
            _taskFluentInterface.Context = this;
            _coreTaskFluentInterface.Context = this;
        }

        public IBuildPropertiesSession Properties { get; }

        public CommandArguments Args { get; }

        public bool IsInteractive { get; set; } = true;

        public void IncreaseDepth()
        {
            _executionDepth++;
        }

        public void LogInfo(string message)
        {
            _log?.LogInformation(message);
        }

        public void LogError(string message)
        {
            _log?.LogError(message);
        }

        public ITaskFluentInterface Tasks()
        {
            return _taskFluentInterface;
        }

        public ICoreTaskFluentInterface CoreTasks()
        {
            return _coreTaskFluentInterface;
        }

        public void DecreaseDepth()
        {
            _executionDepth--;
        }

        public void Fail(string message, int errorCode = 0)
        {
            LogError(message);
            throw new TaskExecutionException(message, errorCode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }

            _disposed = true;
        }
    }
}