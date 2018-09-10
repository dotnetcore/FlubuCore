using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks
{
    public class TaskValidationException : FlubuException
    {
        public TaskValidationException(string message)
            : base(message)
        {
        }

        public TaskValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
