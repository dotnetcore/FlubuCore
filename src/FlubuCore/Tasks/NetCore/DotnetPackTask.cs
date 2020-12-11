using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetPackTask : ExecuteDotnetTaskBase<DotnetPackTask>
    {
        private string _description;

        private string _projectName;

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
            _projectName = projectName;
            return this;
        }

        /// <summary>
        /// The target runtime to restore packages for.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime")]
        public DotnetPackTask Runtime(string runtime)
        {
            WithArgumentsKeyFromAttribute(runtime);
            return this;
        }

        /// <summary>
        /// The target runtime to restore packages for.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime")]
        public DotnetPackTask Runtime(Runtime runtime)
        {
            WithArgumentsKeyFromAttribute(runtime.ToString());
            return this;
        }

        /// <summary>
        /// Directory in which to place built packages.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [ArgKey("--output", "-o")]
        public DotnetPackTask OutputDirectory(string directory)
        {
            WithArgumentsKeyFromAttribute(directory);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetPackTask Configuration(string configuration)
        {
            WithArgumentsKeyFromAttribute(configuration);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetPackTask Configuration(Configuration configuration)
        {
            WithArgumentsKeyFromAttribute(configuration.ToString());
            return this;
        }

        /// <summary>
        /// Include packages with symbols in addition to regular packages in output directory.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--include-symbols")]
        public DotnetPackTask IncludeSymbols()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Include PDBs and source files. Source files go into the src folder in the resulting nuget package.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--include-source")]
        public DotnetPackTask IncludeSource()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Defines the value for the $(VersionSuffix) property in the project.
        /// </summary>
        /// <param name="versionSufix"></param>
        /// <returns></returns>
        [ArgKey("--version-suffix")]
        public DotnetPackTask VersionSuffix(string versionSuffix)
        {
            if (!string.IsNullOrEmpty(versionSuffix))
            {
                WithArgumentsKeyFromAttribute(versionSuffix);
            }

            return this;
        }

        /// <summary>
        /// Set's package version
        /// </summary>
        /// <param name="version">Version prefix e.g. 1.0.0</param>
        /// <param name="versionSuffix">Version suffix e.g. -alpha</param>
        /// <returns></returns>
        [ArgKey("/p:PackageVersion=")]
        public DotnetPackTask PackageVersion(string version, string versionSuffix = null)
        {
            if (!string.IsNullOrEmpty(versionSuffix))
            {
                if (!versionSuffix.StartsWith("-"))
                {
                    versionSuffix = versionSuffix.Insert(0, "-");
                }

                WithArguments($"{GetFirstKeyFromAttribute()}{version}{versionSuffix}");
            }
            else
            {
                WithArguments($"{GetFirstKeyFromAttribute()}{version}");
            }

            return this;
        }

        /// <summary>
        ///  Skip building the project prior to packing. By default, the project will be built.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-build")]
        public DotnetPackTask NoBuild()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set the serviceable flag in the package. For more information, please see https://aka.ms/nupkgservicing.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--serviceable")]
        public DotnetPackTask Servicable()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-dependencies")]
        public DotnetPackTask NoDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
        public DotnetPackTask Force()
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
        public DotnetPackTask Verbosity(VerbosityOptions verbosity)
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

            if (!args.Exists(x => x == "-c" || x == "--configuration"))
            {
                var configuration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
                if (configuration != null)
                {
                    Configuration(configuration);
                }
            }

            base.BeforeExecute(context, runProgramTask);
        }
    }
}
