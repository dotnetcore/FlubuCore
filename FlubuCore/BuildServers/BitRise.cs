using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class BitRise
    {
        public static bool IsRunningOnBitrise => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BITRISE_BUILD_URL"));

        public static string BuildNumber => Environment.GetEnvironmentVariable("BITRISE_BUILD_NUMBER");

        public static string BuildWorkingFolder => Environment.GetEnvironmentVariable("BITRISE_SOURCE_DIR");

        public static string DeployFolder => Environment.GetEnvironmentVariable("BITRISE_DEPLOY_DIR");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public static string CommitId => Environment.GetEnvironmentVariable("BITRISE_GIT_COMMIT");
    }
}
