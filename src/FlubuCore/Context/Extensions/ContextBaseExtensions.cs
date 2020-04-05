using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.IO;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using GlobExpressions;

namespace FlubuCore.Context
{
    public static class ContextBaseExtensions
    {
        /// <summary>
        /// Get's Visual studio solution information. if <see cref="solutionFileName"/> is not specified solution file name is readed from <see cref="IBuildPropertiesContext"/> property <see cref="BuildProps.SolutionFileName"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="solutionFileName"></param>
        /// <returns></returns>
        public static VSSolution GetVsSolution(ITaskContext context, string solutionFileName = null)
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
        /// Gets all files matching glob pattern.
        /// See: https://github.com/kthompson/glob for supported pattern expressions and use cases.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FileFullPath> GetFiles(string directory, params string[] globPattern)
        {
            return GetFiles(directory, GlobOptions.None, globPattern);
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// See: https://github.com/kthompson/glob for supported pattern expressions and use cases.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FileFullPath> GetFiles(string directory, GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
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
        public static List<FullPath> GetDirectories(string directory, params string[] globPattern)
        {
            return GetDirectories(directory, GlobOptions.None, globPattern);
        }

        /// <summary>
        /// Gets all directories matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FullPath> GetDirectories(string directory, GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
        {
            var directoryInfo = new DirectoryInfo(directory);
            return globPattern.SelectMany(pattern => Glob.Directories(directoryInfo, pattern, globOptions)).Select(x => new FullPath(x.FullName)).ToList();
        }
    }
}