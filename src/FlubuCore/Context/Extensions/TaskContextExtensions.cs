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

        /// <summary>
        /// Get's product root directory.
        /// </summary>
        public static FullPath RootDirectory(this ITaskContext context)
        {
            return new FullPath(context.Properties.GetProductRootDir());
        }
        
        public static FileFullPath OutputDirectory(this ITaskContext context)
        {
            return context.RootDirectory().AddFileName(context.Properties.GetOutputDir());
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FileFullPath> GetFiles(this ITaskContext context, string directory, params string[] globPattern)
        {
           return GetFiles(context, directory, GlobOptions.None, globPattern);
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FileFullPath> GetFiles(this ITaskContext context, string directory, GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
        {
            var directoryInfo = new DirectoryInfo(directory);
            return globPattern.SelectMany(pattern => Glob.Files(directoryInfo, pattern, globOptions)).Select(x => new FileFullPath(x.FullName)).ToList();
        }

        /// <summary>
        /// Gets all directories matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FullPath> GetDirectories(this ITaskContext context, string directory, params string[] globPattern)
        {
            return GetDirectories(context, directory, GlobOptions.None, globPattern);
        }

        /// <summary>
        /// Gets all directories matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FullPath> GetDirectories(this ITaskContext context, string directory, GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
        {
            var directoryInfo = new DirectoryInfo(directory);
            return globPattern.SelectMany(pattern => Glob.Directories(directoryInfo, pattern, globOptions)).Select(x => new FullPath(x.FullName)).ToList();
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
