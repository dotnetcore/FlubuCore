using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetPackTask : ExecuteDotnetTaskBase<DotnetPackTask>
    {
        private string _description;

        public DotnetPackTask()
            : base(StandardDotnetCommands.Pack)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return "Executes dotnet command Pack";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// The project to pack, defaults to the project file in the current directory. Can be a path to any project file
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DotnetPackTask Project(string projectName)
        {
            Arguments.Insert(0, projectName);
            return this;
        }

        /// <summary>
        /// Directory in which to place built packages.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public DotnetPackTask OutputDirectory(string directory)
        {
            directory.MustNotBeNullOrEmpty(ValidationMessages.ParamNotNullOrEmpty, nameof(directory));
            WithArguments("-o", directory);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public DotnetPackTask Configuration(string configuration)
        {
            configuration.MustNotBeNullOrEmpty(ValidationMessages.ParamNotNullOrEmpty, nameof(configuration));
            WithArguments("-c", configuration);
            return this;
        }

        /// <summary>
        /// Include packages with symbols in addition to regular packages in output directory.
        /// </summary>
        /// <returns></returns>
        public DotnetPackTask IncludeSymbols()
        {
            WithArguments("--include-symbols");
            return this;
        }

        /// <summary>
        /// Include PDBs and source files. Source files go into the src folder in the resulting nuget package.
        /// </summary>
        /// <returns></returns>
        public DotnetPackTask IncludeSource()
        {
            WithArguments("--include-source");
            return this;
        }

        /// <summary>
        ///  Defines the value for the $(VersionSuffix) property in the project.
        /// </summary>
        /// <param name="versionSufix"></param>
        /// <returns></returns>
        public DotnetPackTask VersionSufix(string versionSufix)
        {
            versionSufix.MustNotBeNullOrEmpty(ValidationMessages.ParamNotNullOrEmpty, nameof(versionSufix));
            WithArguments("--version-suffix", versionSufix);
            return this;
        }

        /// <summary>
        ///  Skip building the project prior to packing. By default, the project will be built.
        /// </summary>
        /// <returns></returns>
        public DotnetPackTask NoBuild()
        {
            WithArguments("--no-build");
            return this;
        }

        /// <summary>
        /// Set the serviceable flag in the package. For more information, please see https://aka.ms/nupkgservicing.
        /// </summary>
        /// <returns></returns>
        public DotnetPackTask Servicable()
        {
            WithArguments("--serviceable");
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        public DotnetPackTask NoDependencies()
        {
            WithArguments("--no-dependencies");
            return this;
        }

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        public DotnetPackTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public DotnetPackTask Verbosity(VerbosityOptions verbosity)
        {
            WithArguments("--verbosity", verbosity.ToString().ToLower());
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context)
        {
            if (!Arguments.Exists(x => x == "-c" || x == "--configuration"))
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
