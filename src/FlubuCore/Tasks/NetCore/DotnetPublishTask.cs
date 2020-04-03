using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    /// <summary>
    /// Publishes a .NET project for deployment (including the runtime).
    /// </summary>
    public class DotnetPublishTask : ExecuteDotnetTaskBase<DotnetPublishTask>
    {
        private string _description;

        private string _projectName;

        public DotnetPublishTask()
            : base(StandardDotnetCommands.Publish)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return "Executes dotnet command Publish";
                }

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        /// The MSBuild project file to publish. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DotnetPublishTask Project(string projectName)
        {
            _projectName = projectName;
            return this;
        }

        /// <summary>
        /// Target framework to publish for. The target framework has to be specified in the project file.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetPublishTask Framework(string framework)
        {
            WithArgumentsKeyFromAttribute(framework);
            return this;
        }

        /// <summary>
        /// Target runtime to publish for. The default is to build a portable application.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime", "-r")]
        public DotnetPublishTask AddRuntime(string runtime)
        {
            WithArgumentsKeyFromAttribute(runtime);
            return this;
        }

        /// <summary>
        /// Target runtime to publish for. The default is to build a portable application.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        [ArgKey("--runtime", "-r")]
        public DotnetPublishTask AddRuntime(Runtime runtime)
        {
            WithArgumentsKeyFromAttribute(runtime.ToString());
            return this;
        }

        /// <summary>
        /// Output directory in which to place the published artifacts.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [ArgKey("--output", "-o")]
        public DotnetPublishTask OutputDirectory(string directory)
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
        public DotnetPublishTask Configuration(string configuration)
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
        public DotnetPublishTask Configuration(Configuration configuration)
        {
            WithArgumentsKeyFromAttribute(configuration.ToString());
            return this;
        }

        /// <summary>
        /// Set the --version-suffix. Set the value of the $(VersionSuffix) property to use when building the project.
        /// </summary>
        /// <param name="suffix">version suffix e.g. -alpha</param>
        /// <returns></returns>
        [ArgKey("--version-suffix")]
        public DotnetPublishTask VersionSuffix(string suffix)
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
        [ArgKey("/p:Version=")]
        public DotnetPublishTask Version(string version, string versionSuffix = null)
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
        [ArgKey("/p:InformationalVersion=")]
        public DotnetPublishTask InformationalVersion(string version)
        {
            WithArguments($"/p:InformationalVersion={version}");
            return this;
        }

        /// <summary>
        /// Set assembly version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("/p:AssemblyVersion==")]
        public DotnetPublishTask AssemblyVersion(string version)
        {
            WithArguments($"/p:AssemblyVersion={version}");
            return this;
        }

        /// <summary>
        /// Set file version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("/p:FileVersion=")]
        public DotnetPublishTask FileVersion(string version)
        {
            WithArguments($"/p:FileVersion={version}");
            return this;
        }

        /// <summary>
        ///  Set this flag to ignore project to project references and only restore the root project
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-dependencies")]
        public DotnetPublishTask NoDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set this flag to force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
        public DotnetPublishTask Force()
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
        public DotnetPublishTask Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString().ToLower());
            return this;
        }

        /// <summary>
        ///  Publish the .NET Core runtime with your application so the runtime doesn't need to be installed on the target machine.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--self-contained")]
        public DotnetPublishTask SelfContained()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Do not build the project before publishing. Implies --no-restore.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-build")]
        public DotnetPublishTask NoBuild()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Do not restore the project before building.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-restore")]
        public DotnetPublishTask NoRestore()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Publish application as a single executable for specified platform. Available only for .netcoreapp 3.0 and above.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/p:PublishSingleFile")]
        public DotnetPublishTask PublishSingleFile()
        {
            WithArguments("/p:PublishSingleFile=true");
            return this;
        }

        /// <summary>
        /// Reduces the size of the executable to a great extent and create a trimmed self-contained single executable by removing unused dlls. Available only for .netcoreapp 3.0 and above.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/p:PublishTrimmed")]
        public DotnetPublishTask PublishTrimmedFile()
        {
            WithArguments("/p:PublishTrimmed=true");
            return this;
        }

        /// <summary>
        /// Increases self-contained single executable app startup time. Available only in.netcoreapp 3.0 and above.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/p:PublishReadyToRun")]
        public DotnetPublishTask PublishReadyToRun()
        {
            WithArguments("/p:PublishReadyToRun=true");
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
