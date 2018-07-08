using System;
using System.Diagnostics;
using System.Threading;

namespace FlubuCore.Context
{
    public static class TaskContextExtensions
    {
        public static string GetEnvironmentVariable(this ITaskContext context, string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        public static void WaitForDebugger(this ITaskContext context)
        {
            context.LogInfo("Waiting for debugger to be attached.");

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }

            context.LogInfo("Debugger attached.");
        }
    }
}
