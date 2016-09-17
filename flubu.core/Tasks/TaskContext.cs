using flubu.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using flubu.Scripting;
using System.Diagnostics;

namespace flubu
{
    public class TaskContext : ITaskContext
    {
        public TaskContext (ITaskContextProperties properties, CommandArguments args)
        {
            this.Properties = properties;
            this.Args = args;
        }

        public CommandArguments Args { get; }

        public bool IsInteractive { get; set; } = true;

        public ITaskContextProperties Properties { get; }

        public TaskContext AddLogger (ILogger logger)
        {
            _log = logger;
            return this;
        }

        public void IncreaseDepth()
        {
            executionDepth++;
        }

        public void ResetDepth()
        {
            executionDepth = 0;
        }

        public void WriteMessage(TaskMessageLevel level, string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            _log.LogInformation(message);
        }

        public void DecreaseDepth()
        {
            executionDepth--;
        }

        public void Fail(string message)
        {
            WriteMessage(TaskMessageLevel.Error, message);
            throw new TaskExecutionException(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed || !disposing)
                return;

            disposed = true;
        }
        private bool disposed;
        private int executionDepth;
        private ILogger _log;
        private ITaskContextProperties taskContextProperties;
        private CommandArguments args;
    }
}