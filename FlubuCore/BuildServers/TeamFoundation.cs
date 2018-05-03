using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class TeamFoundation
    {
        public static bool IsRunningOnTFS
        {
            get
            {
               var isHostedAgent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGENT_NAME")) &&
                    Environment.GetEnvironmentVariable("AGENT_NAME") == "Hosted Agent";
                return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) && !isHostedAgent;
            }
        }

        public static string BuildNumber => Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");
    }
}
