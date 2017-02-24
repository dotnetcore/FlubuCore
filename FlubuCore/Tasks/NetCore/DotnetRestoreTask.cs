using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetRestoreTask : ExecuteDotnetTask
    {
        public DotnetRestoreTask() : base(StandardDotnetCommands.Restore)
        {
        }

        public DotnetRestoreTask Project(string projectName)
        {
            WithArguments(projectName);
            return this;
        }

        /// <summary>
        /// Add a NuGet package source to use during the restore.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public DotnetRestoreTask AddNugetSouce(string source)
        {
            WithArguments("-s", source);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public DotnetRestoreTask AddRuntime(string runtime)
        {
            WithArguments("-r", runtime);
            return this;
        }

        /// <summary>
        /// Directory to install packages in.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public DotnetRestoreTask PackagesDirectory(string directory)
        {
            WithArguments("--pacakges", directory);
            return this;
        }

        /// <summary>
        /// Disables restoring multiple projects in parallel.
        /// </summary>
        /// <returns></returns>
        public DotnetRestoreTask DisableParallel()
        {
            WithArguments("--disable-parallel");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public DotnetRestoreTask NugetConfigFile(string configFile)
        {
            WithArguments("--configfile", configFile);
            return this;
        }

        /// <summary>
        /// Do not cache packages and http requests.
        /// </summary>
        /// <returns></returns>
        public DotnetRestoreTask NoCache()
        {
            WithArguments("--no-cache");
            return this;
        }

        /// <summary>
        /// Treat package source failures as warnings.
        /// </summary>
        /// <returns></returns>
        public DotnetRestoreTask IgnoreFailedSources()
        {
            WithArguments("--ignore-failed-sources");
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        public DotnetRestoreTask NoDependencies()
        {
            WithArguments("--no-dependencies");
            return this;
        }
    }
}
