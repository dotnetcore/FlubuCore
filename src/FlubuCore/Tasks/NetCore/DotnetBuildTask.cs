using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    /// <summary>
    /// Publishes a .NET project for deployment (including the runtime).
    /// </summary>
    public class DotnetBuildTask : ExecuteDotnetTaskBase<DotnetBuildTask>
    {
        private string _description;

        private string _projectName;

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
            _projectName = projectName;
            return this;
        }

        /// <summary>
        /// Compiles for a specific framework. The framework must be defined in the project file.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetBuildTask Framework(string framework)
        {
            WithArgumentsKeyFromAttribute(framework);
            return this;
        }

        /// <summary>
        /// Compiles for a specific framework. The framework must be defined in the project file.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetBuildTask Framework(Framework framework)
        {
            WithArgumentsKeyFromAttribute(framework.ToString());
            return this;
        }

        /// <summary>
        /// Target runtime to build for. The default is to build a portable application.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime", "-r")]
        public DotnetBuildTask AddRuntime(string runtime)
        {
            WithArgumentsKeyFromAttribute(runtime);
            return this;
        }

        /// <summary>
        /// Target runtime to build for. The default is to build a portable application.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime", "-r")]
        public DotnetBuildTask AddRuntime(Runtime runtime)
        {
            WithArgumentsKeyFromAttribute(runtime.ToString());
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetBuildTask Configuration(string configuration)
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
        public DotnetBuildTask Configuration(Configuration configuration)
        {
            WithArgumentsKeyFromAttribute(configuration.ToString());
            return this;
        }

        /// <summary>
        /// The output directory to place built artifacts in.
        /// </summary>
        /// <param name="path">The output path.</param>
        /// <returns></returns>
        [ArgKey("--output", "-o")]
        public DotnetBuildTask OutputDirectory(string path)
        {
            WithArgumentsKeyFromAttribute(path);
            return this;
        }

        /// <summary>
        ///  Disables incremental build.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-incremental")]
        public DotnetBuildTask NoIncrementail()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-dependencies")]
        public DotnetBuildTask NoDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
        public DotnetBuildTask Force()
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
        public DotnetBuildTask Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString().ToLower());
            return this;
        }

        /// <summary>
        /// Set the --version-suffix. Set the value of the $(VersionSuffix) property to use when building the project.
        /// </summary>
        /// <param name="suffix">version suffix e.g. -alpha</param>
        /// <returns></returns>
        [ArgKey("--version-suffix")]
        public DotnetBuildTask VersionSuffix(string suffix)
        {
            if (!string.IsNullOrEmpty(suffix))
            {
                WithArgumentsKeyFromAttribute(suffix);
            }

            return this;
        }

        /// <summary>
        /// Set version
        /// </summary>
        /// <param name="version">Version prefix e.g. 1.0.0</param>
        /// <param name="versionSuffix">Version suffix e.g. -alpha</param>
        /// <returns></returns>
        [ArgKey("-p:Version=")]
        public DotnetBuildTask Version(string version, string versionSuffix = null)
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
        /// Set information version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("-p:InformationalVersion=")]
        public DotnetBuildTask InformationalVersion(string version)
        {
            WithArguments($"{GetFirstKeyFromAttribute()}{version}");
            return this;
        }

        /// <summary>
        /// Set assembly version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("-p:AssemblyVersion=")]
        public DotnetBuildTask AssemblyVersion(string version)
        {
            WithArguments($"{GetFirstKeyFromAttribute()}{version}");
            return this;
        }

        /// <summary>
        /// Set file version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("-p:FileVersion=")]
        public DotnetBuildTask FileVersion(string version)
        {
            WithArguments($"{GetFirstKeyFromAttribute()}{version}");
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
