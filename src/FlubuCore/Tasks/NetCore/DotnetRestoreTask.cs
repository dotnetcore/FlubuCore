using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetRestoreTask : ExecuteDotnetTaskBase<DotnetRestoreTask>
    {
        private string _description;

        private string _projectName;

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
            _projectName = projectName;
            return this;
        }

        /// <summary>
        /// Add a NuGet package source to use during the restore.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [ArgKey("--source", "-s")]
        public DotnetRestoreTask AddNugetSouce(string source)
        {
            WithArgumentsKeyFromAttribute(source);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime", "-r")]
        public DotnetRestoreTask AddRuntime(string runtime)
        {
            WithArgumentsKeyFromAttribute(runtime);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime", "-r")]
        public DotnetRestoreTask AddRuntime(Runtime runtime)
        {
            WithArgumentsKeyFromAttribute(runtime.ToString());
            return this;
        }

        /// <summary>
        /// Directory to install packages in.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [ArgKey("--packages")]
        public DotnetRestoreTask PackagesDirectory(string directory)
        {
            WithArgumentsKeyFromAttribute(directory);
            return this;
        }

        /// <summary>
        /// Disables restoring multiple projects in parallel.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--disable-parallel")]
        public DotnetRestoreTask DisableParallel()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        [ArgKey("--configfile")]
        public DotnetRestoreTask NugetConfigFile(string configFile)
        {
            WithArgumentsKeyFromAttribute(configFile);
            return this;
        }

        /// <summary>
        /// Do not cache packages and http requests.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-cache")]
        public DotnetRestoreTask NoCache()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Treat package source failures as warnings.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-failed-sources")]
        public DotnetRestoreTask IgnoreFailedSources()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-dependencies")]
        public DotnetRestoreTask NoDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
        public DotnetRestoreTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("--verbosity", "-v")]
        public DotnetRestoreTask Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString().ToLower());
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context, IRunProgramTask runProgramTask)
        {
            var args = GetArguments();
            if (string.IsNullOrEmpty(_projectName))
            {
                if (args.Count == 0 || args[0].StartsWith("-"))
                {
                    var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                    if (solustionFileName != null)
                    {
                        InsertArgument(0, solustionFileName);
                    }
                }
            }
            else
            {
                InsertArgument(0, _projectName);
            }

            base.BeforeExecute(context, runProgramTask);
        }
    }
}
