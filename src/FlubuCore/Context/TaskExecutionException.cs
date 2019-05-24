using System;

namespace FlubuCore.Context
{
    public class TaskExecutionException : FlubuException
    {
        public TaskExecutionException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public TaskExecutionException(string message, int errorCode, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; }
    }
}