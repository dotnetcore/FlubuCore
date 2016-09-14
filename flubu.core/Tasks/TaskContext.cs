using flubu.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace flubu
{
    public class TaskContext : ITaskContext
    {
        public TaskContext (ITaskContextProperties properties, IEnumerable<string> args)
        {
            this.properties = properties;
            this.args = new List<string>(args);
        }

        public IList<string> Args
        {
            get { return args; }
        }

        public bool IsInteractive
        {
            get { return isInteractive; }
            set { isInteractive = value; }
        }

        public ITaskContextProperties Properties
        {
            get { return properties; }
        }

        public TaskContext AddLogger (ILogger logger)
        {
            loggers.Add(logger);
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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void WriteMessage(TaskMessageLevel level, string message)
        {
            //todo implement with Ms logger
            throw new NotImplementedException();

            //foreach (ILogger logger in loggers)
            //{
            //    try
            //    {
            //        logger.WriteMessage(level, executionDepth, message);
            //    }
            //    catch
            //    {
            //    }
            //}
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

        private readonly List<string> args;
        private bool disposed;
        private int executionDepth;
        private readonly List<ILogger> loggers = new List<ILogger>();
        private readonly ITaskContextProperties properties;
        private bool isInteractive = true;
    }
}