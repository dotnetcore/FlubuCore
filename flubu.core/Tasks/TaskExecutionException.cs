using System;
using System.Globalization;

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

        public TaskExecutionException(string formatMessage, params object[] arguments)
            : base(string.Format(CultureInfo.InvariantCulture, formatMessage, arguments))
        {
        }

        public TaskExecutionException(string formatMessage, object first)
            : base(string.Format(CultureInfo.InvariantCulture, formatMessage, first))
        {
        }

        public TaskExecutionException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
