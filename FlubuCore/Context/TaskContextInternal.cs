using System;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContextInternal : TaskContext, ITaskContextInternal
    {
        private bool _disposed;

        private int _executionDepth;

        public TaskContextInternal(
            ILogger log,
            IBuildPropertiesSession taskContextProperties,
            CommandArguments args,
            TargetTree targetTree,
            ITaskFactory taskFactory,
            ICoreTaskFluentInterface coreTaskFluentInterface,
            ITaskFluentInterface taskFluentInterface,
            ITargetFluentInterface targetFluent)
            : base(log, taskFactory, coreTaskFluentInterface, taskFluentInterface, targetFluent, targetTree, taskContextProperties)
        {
            Args = args;
        }

        public CommandArguments Args { get; }

        public bool IsInteractive { get; set; } = true;

        public void IncreaseDepth()
        {
            _executionDepth++;
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