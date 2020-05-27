using System;

namespace FlubuCore.BuildServers
{
    public class BitRise
    {
        public static bool RunningOnBitrise => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BITRISE_BUILD_URL"));

        /// <summary>
        /// Indicates whether build is running on Bitrise build server.
        /// </summary>
        public bool IsRunningOnBitrise => RunningOnBitrise;

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("BITRISE_BUILD_NUMBER");

        public string BuildWorkingFolder => Environment.GetEnvironmentVariable("BITRISE_SOURCE_DIR");

        public string DeployFolder => Environment.GetEnvironmentVariable("BITRISE_DEPLOY_DIR");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public string CommitId => Environment.GetEnvironmentVariable("BITRISE_GIT_COMMIT");
    }
}
