using System;

namespace FlubuCore.BuildServers
{
    public class TeamFoundation
    {
        public static bool RunningOnTFS
        {
            get
            {
                var isHostedAgent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGENT_NAME")) &&
                                    Environment.GetEnvironmentVariable("AGENT_NAME") == "Hosted Agent";
                return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) && !isHostedAgent;
            }
        }

        /// <summary>
        /// Indicates whether build is running on Team foundation server.
        /// </summary>
        public bool IsRunningOnTFS => RunningOnTFS;

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");
    }
}
