using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using FlubuCore.IO;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using GlobExpressions;

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

        public static FileFullPath GetOutputDirectory(this ITaskContext context)
        {
            return context.GetRootDirectory().AddFileName(context.Properties.GetOutputDir());
        }

        /// <summary>
        /// Local git repository information.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Git Git(this ITaskContext context)
        {
            return new Git(context);
        }
    }
}
