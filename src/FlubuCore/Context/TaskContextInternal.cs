using System;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Context
{
    public class TaskContextInternal : TaskContext, ITaskContextInternal
    {
        private bool _disposed;

        public TaskContextInternal(
            ILogger log,
            IBuildPropertiesSession taskContextProperties,
            CommandArguments args,
            TargetTree targetTree,
            IBuildServer buildServers,
            ITaskFactory taskFactory,
            IFluentInterfaceFactory fluentFactory)
            : base(log, taskFactory, fluentFactory, targetTree, buildServers, taskContextProperties)
        {
            Args = args;
        }

        public CommandArguments Args { get; }

        public string FlubuHelpText { get; set; }

        public void Fail(string message, int errorCode = 0)
        {
            LogError(message);
            throw new TaskExecutionException(message, errorCode);
        }

        public void IncreaseDepth()
        {
            ExecutionDepth++;
        }

        public void DecreaseDepth()
        {
            if (ExecutionDepth > 0)
            {
                ExecutionDepth--;
            }
        }

        public void ResetDepth()
        {
            ExecutionDepth = 0;
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