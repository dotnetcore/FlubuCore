using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetNugetPushTask : ExecuteDotnetTask
    {
        /// <summary>
        /// Pushes the nuget package to nuget server.
        /// </summary>
        /// <param name="packagePath">Path to nupkg</param>
        public DotnetNugetPushTask(string packagePath) : base("nuget push")
        {
            WithArguments(packagePath);
        }

        /// <summary>
        ///  Specifies the server URL
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <returns></returns>
        public DotnetNugetPushTask ServerUrl(string serverUrl)
        {
            WithArguments("-s", serverUrl);
            return this;
        }

        /// <summary>
        ///  Specifies the symbol server URL. If not specified, nuget.smbsrc.net is used when pushing to nuget.org.
        /// </summary>
        /// <param name="symbolServerUrl"></param>
        /// <returns></returns>
        public DotnetNugetPushTask SymbolServerUrl(string symbolServerUrl)
        {
            WithArguments("-ss", symbolServerUrl);
            return this;
        }

        /// <summary>
        /// The API key for the server.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public DotnetNugetPushTask ApiKey(string apiKey)
        {
            WithArguments("-k", apiKey);
            return this;
        }

        /// <summary>
        /// The API key for the symbol server.
        /// </summary>
        /// <param name="symbolApyKey"></param>
        /// <returns></returns>
        public DotnetNugetPushTask SymbolApiKey(string symbolApyKey)
        {
            WithArguments("-sk", symbolApyKey);
            return this;
        }

        /// <summary>
        /// Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes). Specifying 0 (zero seconds) applies the default value.
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public DotnetNugetPushTask Timeout(int timeoutInSeconds)
        {
            WithArguments("-t", timeoutInSeconds.ToString());
            return this;
        }
    }
}
