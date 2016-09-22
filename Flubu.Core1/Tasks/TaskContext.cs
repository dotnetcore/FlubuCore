using System;
using System.Diagnostics;
using Flubu.Scripting;
using Microsoft.Extensions.Logging;

namespace Flubu.Tasks
{
    public class TaskContext : ITaskContext
    {
        private ILogger _log;

        private bool _disposed;

        private int _executionDepth;

        public TaskContext(CommandArguments args)
        {
            Args = args;
        }

        public CommandArguments Args { get; }

        public bool IsInteractive { get; set; } = true;

        public void IncreaseDepth()
        {
            _executionDepth++;
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            _log.LogInformation(message);
        }

        public void DecreaseDepth()
        {
            _executionDepth--;
        }

        public void Fail(string message)
        {
            WriteMessage(message);
            throw new TaskExecutionException(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TaskContext AddLogger(ILogger logger)
        {
            _log = logger;
            return this;
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