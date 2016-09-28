using System;

namespace Flubu.Context
{
    public class TaskExecutionException : Exception
    {
        public TaskExecutionException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; private set; }
    }
}