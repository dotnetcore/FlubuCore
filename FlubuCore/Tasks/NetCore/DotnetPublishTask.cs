using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    /// <summary>
    /// Publishes a .NET project for deployment (including the runtime).
    /// </summary>
    public class DotnetPublishTask : ExecuteDotnetTaskBase<DotnetPublishTask>
    {
        private string _description;

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
            Arguments.Insert(0, projectName);
            return this;
        }

        /// <summary>
        /// Target framework to publish for. The target framework has to be specified in the project file.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        public DotnetPublishTask Framework(string framework)
        {
            WithArguments("-f", framework);
            return this;
        }

        /// <summary>
        /// Target runtime to publish for. The default is to build a portable application.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public DotnetPublishTask AddRuntime(string runtime)
        {
            WithArguments("-r", runtime);
            return this;
        }

        /// <summary>
        /// Output directory in which to place the published artifacts.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public DotnetPublishTask OutputDirectory(string directory)
        {
            WithArguments("-o", directory);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public DotnetPublishTask Configuration(string configuration)
        {
            WithArguments("-c", configuration);
            return this;
        }

        public DotnetPublishTask VersionSufix(string versionSufix)
        {
            WithArguments("--version-suffix", versionSufix);
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
