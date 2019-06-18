using System;
using System.Diagnostics;
using System.Threading;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

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

        /// <summary>
        /// Get's Visual studio solution information. if <see cref="solutionFileName"/> is not specified solution file name is readed from <see cref="IBuildPropertiesContext"/> property <see cref="BuildProps.SolutionFileName"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="solutionFileName"></param>
        /// <returns></returns>
        public static VSSolution GetVsSolution(this ITaskContext context, string solutionFileName = null)
        {
            VSSolution solution = null;
            bool saveSolution = true;
            if (string.IsNullOrEmpty(solutionFileName))
            {
                solutionFileName = context.Properties.TryGet<string>(BuildProps.SolutionFileName);
                solution = context.Properties.TryGet<VSSolution>(BuildProps.Solution);
            }
            else
            {
                saveSolution = false;
            }

            if (solution == null)
            {
               solution = context.Tasks().LoadSolutionTask(solutionFileName).DoNotFailOnError().Execute(context);
            }

            if (saveSolution)
            {
                context.Properties.Set(BuildProps.Solution, solution);
            }

            return solution;
        }
    }
}
