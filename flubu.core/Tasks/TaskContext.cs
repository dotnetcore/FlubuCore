using System;
using System.Diagnostics;
using Flubu.Scripting;
using Microsoft.Extensions.Logging;

namespace Flubu.Tasks
{
    public class TaskContext : ITaskContext
    {
        private ILogger log;

        private bool disposed;

        private int executionDepth;

        public TaskContext(CommandArguments args)
        {
            Args = args;
        }

        public CommandArguments Args { get; }

        public bool IsInteractive { get; set; } = true;

        public void IncreaseDepth()
        {
            executionDepth++;
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            log.LogInformation(message);
        }

        public void DecreaseDepth()
        {
            executionDepth--;
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
            log = logger;
            return this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed || !disposing)
            {
                return;
            }

            disposed = true;
        }
    }
}