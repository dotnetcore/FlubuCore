using System;

namespace Flubu.Tasks
{
    public class TaskExecutionException : Exception
    {
        public TaskExecutionException()
        {
        }

        public TaskExecutionException(string message)
            : base(message)
        {
        }
    }
}