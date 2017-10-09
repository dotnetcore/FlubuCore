using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    public static class TaskContextExtensions
    {
        public static string GetEnvironmentVariable(this ITaskContext context, string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
    }
}
