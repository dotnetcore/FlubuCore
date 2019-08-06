using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetNugetPushTask : ExecuteDotnetTaskBase<DotnetNugetPushTask>
    {
        private string _description;
        private bool _skipPushOnLocalBuild;
        private string _packagePath;

        /// <summary>
        /// Pushes the nuget package to nuget server.
        /// </summary>
        /// <param name="packagePath">Path to nupkg</param>
        public DotnetNugetPushTask(string packagePath)
            : base("nuget")
        {
            WithArguments("push");
            WithArguments(packagePath);
            _packagePath = packagePath;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes dotnet command Nuget Push";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        ///  Specifies the server URL (source).
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <returns></returns>
        [ArgKey("-s", "--source")]
        public DotnetNugetPushTask ServerUrl(string serverUrl)
        {
            WithArgumentsKeyFromAttribute(serverUrl);
            return this;
        }

        /// <summary>
        ///  Specifies the symbol server URL(symbol source). If not specified, nuget.smbsrc.net is used when pushing to nuget.org.
        /// </summary>
        /// <param name="symbolServerUrl"></param>
        /// <returns></returns>
        [ArgKey("-ss", "--symbol-source")]
        public DotnetNugetPushTask SymbolServerUrl(string symbolServerUrl)
        {
            WithArgumentsKeyFromAttribute(symbolServerUrl);
            return this;
        }

        /// <summary>
        /// If applied pushing packages to nuget repository is disabled on local build.
        /// </summary>
        /// <returns></returns>
        public DotnetNugetPushTask SkipPushOnLocalBuild()
        {
            _skipPushOnLocalBuild = true;
            return this;
        }

        /// <summary>
        /// The API key for the server.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [ArgKey("-k", "--api-key")]
        public DotnetNugetPushTask ApiKey(string apiKey)
        {
            WithArgumentsKeyFromAttribute(apiKey, true);
            return this;
        }

        /// <summary>
        /// The API key for the symbol server.
        /// </summary>
        /// <param name="symbolApiKey"></param>
        /// <returns></returns>
        [ArgKey("-sk", "--symbol-api-key")]
        public DotnetNugetPushTask SymbolApiKey(string symbolApiKey)
        {
            WithArgumentsKeyFromAttribute(symbolApiKey, true);
            return this;
        }

        /// <summary>
        /// Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes). Specifying 0 (zero seconds) applies the default value.
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        [ArgKey("-t", "--timeout")]
        public DotnetNugetPushTask Timeout(int timeoutInSeconds)
        {
            WithArgumentsKeyFromAttribute(timeoutInSeconds.ToString());
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _packagePath.MustNotBeNullOrEmpty("packagePath (path to .nupkg) must not be null or empty.");

            // do not push new packages from a local build
            if (context.BuildSystems().IsLocalBuild && _skipPushOnLocalBuild)
            {
                context.LogInfo("pushing package on local build is disabled in build script...Skiping.");
                return 1;
            }

            return base.DoExecute(context);
        }
    }
}
