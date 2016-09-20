using flubu.Scripting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace flubu
{
    public interface ITaskContext : IDisposable
    {
        CommandArguments Args { get; }

        bool IsInteractive { get; }

        void DecreaseDepth();

        void Fail(string message);

        void IncreaseDepth();

        void WriteMessage(string message);
    }

    public class TaskContext : ITaskContext
    {
        public TaskContext(CommandArguments args)
        {
            this.Args = args;
        }

        public CommandArguments Args { get; }

        public bool IsInteractive { get; set; } = true;

        public TaskContext AddLogger(ILogger logger)
        {
            _log = logger;
            return this;
        }

        public void IncreaseDepth()
        {
            executionDepth++;
        }

        public void WriteMessage(string message)
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
            WriteMessage(message);
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
        private CommandArguments args;
    }
}