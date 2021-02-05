using System;

namespace FlubuCore.BuildServers
{
    public class GoCD
    {
        public static bool RunningOnGoCD => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GO_SERVER_URL"));

        /// <summary>
        /// Indicates whether build is running on GoCd build server.
        /// </summary>
        public bool IsRunningOnGoCD => RunningOnGoCD;

        /// <summary>
        /// The commitId.
        /// </summary>
        public string CommitId => Environment.GetEnvironmentVariable("GO_TO_REVISION");
    }
}
