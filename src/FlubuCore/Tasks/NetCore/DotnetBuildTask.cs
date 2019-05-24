using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    /// <summary>
    /// Publishes a .NET project for deployment (including the runtime).
    /// </summary>
    public class DotnetBuildTask : ExecuteDotnetTaskBase<DotnetBuildTask>
    {
        private string _description;

        public DotnetBuildTask()
            : base(StandardDotnetCommands.Build)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes dotnet command Build.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// The MSBuild project file to build. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DotnetBuildTask Project(string projectName)
        {
            InsertArgument(0, projectName);
            return this;
        }

        public DotnetBuildTask Framework(string framework)
        {
            WithArgumentsValueRequired("-f", framework);
            return this;
        }

        /// <summary>
        /// Target runtime to build for. The default is to build a portable application.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public DotnetBuildTask AddRuntime(string runtime)
        {
            WithArgumentsValueRequired("-r", runtime);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public DotnetBuildTask Configuration(string configuration)
        {
            WithArgumentsValueRequired("-c", configuration);
            return this;
        }

        /// <summary>
        ///  Disables incremental build.
        /// </summary>
        /// <returns></returns>
        public DotnetBuildTask NoIncrementail()
        {
            WithArguments("--no-incremental");
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        public DotnetBuildTask NoDependencies()
        {
            WithArguments("--no-dependencies");
            return this;
        }

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        public DotnetBuildTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public DotnetBuildTask Verbosity(VerbosityOptions verbosity)
        {
            WithArguments("--verbosity", verbosity.ToString().ToLower());
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context)
        {
            var args = GetArguments();
            if (args.Count == 0 || args[0].StartsWith("-"))
            {
                var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                if (solustionFileName != null)
                {
                    Project(solustionFileName);
                }
            }

            if (!args.Exists(x => x == "-c" || x == "--configuration"))
            {
                var configuration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
                if (configuration != null)
                {
                    Configuration(configuration);
                }
            }
        }
    }
}
