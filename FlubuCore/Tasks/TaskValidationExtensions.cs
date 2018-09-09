using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    internal static class TaskExtensions
    {
        public static void MustNotBeNullOrEmpty(this string parameter, string validationMessage)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new TaskExecutionException(validationMessage, 0);
            }
        }
    }
}
