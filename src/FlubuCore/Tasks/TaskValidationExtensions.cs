using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    internal static class TaskExtensions
    {
        public static void MustNotBeNullOrEmpty(this string parameter, string validationMessage, params string[] messageArgs)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new TaskValidationException(string.Format(validationMessage, messageArgs));
            }
        }
    }
}
