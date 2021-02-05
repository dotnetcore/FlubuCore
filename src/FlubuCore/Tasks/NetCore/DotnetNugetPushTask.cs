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
        [ArgKey("--source", "-s")]
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
        [ArgKey("--symbol-source", "-ss")]
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
        [ArgKey("--api-key", "-k")]
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
        [ArgKey("--symbol-api-key", "-sk")]
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
        [ArgKey("--timeout", "-t")]
        public DotnetNugetPushTask Timeout(int timeoutInSeconds)
        {
            WithArgumentsKeyFromAttribute(timeoutInSeconds.ToString());
            return this;
        }

        /// <summary>
        /// If a symbols package exists, it will not be pushed to a symbols server.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-symbols")]
        public DotnetNugetPushTask NoSymbols()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Disable buffering when pushing to an HTTP(S) server to decrease memory usage.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--disable-buffering")]
        public DotnetNugetPushTask DisableBuffering()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Does not append "api/v2/package" to the source URL.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-service-endpoint")]
        public DotnetNugetPushTask NoServiceEndpoint()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Allow the command to block and require manual action for operations like authentication.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--interactive")]
        public DotnetNugetPushTask Interactive()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// If a package and version already exists, skip it and continue with the next package in the push, if any.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-duplicate")]
        public DotnetNugetPushTask SkipDuplicate()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _packagePath.MustNotBeNullOrEmpty("packagePath (path to .nupkg) must not be null or empty.");

            // do not push new packages from a local build
            if (context.BuildServers().IsLocalBuild && _skipPushOnLocalBuild)
            {
                context.LogInfo("pushing package on local build is disabled in build script...Skiping.");
                return 1;
            }

            return base.DoExecute(context);
        }
    }
}
