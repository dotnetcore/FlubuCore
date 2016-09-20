using System;

namespace flubu
{
    public class TaskExecutionException : Exception
    {
        public TaskExecutionException ()
        {
        }

        public TaskExecutionException (string message) : base (message)
        {
        }
    }
}
