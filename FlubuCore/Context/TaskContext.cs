using System;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContext : ITaskContext
    {
        private readonly ILogger _log;
        private readonly ITaskFactory _taskFactory;

        private bool _disposed;

        private int _executionDepth;

        public TaskContext(
            ILogger log,
            ITaskContextSession taskContextProperties,
            CommandArguments args,
            ITaskFactory taskFactory)
        {
            _log = log;
            _taskFactory = taskFactory;
            Args = args;
            Properties = taskContextProperties;
        }

        public ITaskContextSession Properties { get; }

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

        public TaskFluentInterface Tasks()
        {
            return new TaskFluentInterface(this);
        }

        public CoreTaskFluentInterface CoreTasks()
        {
           return new CoreTaskFluentInterface(this);
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

        public T CreateTask<T>()
            where T : TaskBase
        {
            return _taskFactory.Create<T>();
        }

        public T CreateTask<T>(params object[] constructorArgs)
            where T : TaskBase
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