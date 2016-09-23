using System;

namespace Flubu.Tasks
{
    public class TaskExecutionException : Exception
    {
        public TaskExecutionException(string message)
            : base(message)
        {
        }

        public TaskExecutionException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; private set; }
    }
}