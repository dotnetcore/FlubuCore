using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetRestoreTask : ExecuteDotnetTaskBase<DotnetRestoreTask>
    {
        private string _description;

        public DotnetRestoreTask()
            : base(StandardDotnetCommands.Restore)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return "Executes dotnet command Restore";
                }

                return _description;
            }

            set { _description = value; }
        }

        public DotnetRestoreTask Project(string projectName)
        {
            Arguments.Insert(0, projectName);
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

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        public DotnetRestoreTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public DotnetRestoreTask Verbosity(VerbosityOptions verbosity)
        {
            WithArguments("--verbosity", verbosity.ToString().ToLower());
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context)
        {
            if (Arguments.Count == 0 || Arguments[0].StartsWith("-"))
            {
                var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                if (solustionFileName != null)
                {
                    Project(solustionFileName);
                }
            }
        }
    }
}
