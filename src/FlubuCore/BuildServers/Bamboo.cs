using System;

namespace FlubuCore.BuildServers
{
    public class Bamboo
    {
        public static bool RunningOnBamboo => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("bamboo_buildNumber"));

        /// <summary>
        /// Indicates whether build is running on Bamboo build server.
        /// </summary>
        public bool IsRunningOnBamboo => RunningOnBamboo;

        /// <summary>
        /// Gets the build id.
        /// </summary>
        public string BuildKey => Environment.GetEnvironmentVariable("bamboo_buildKey");

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("bamboo_buildNumber");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public string BuildWorkingFolder => Environment.GetEnvironmentVariable("bamboo_build_working_directory");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public string CommitId => Environment.GetEnvironmentVariable("bamboo_planRepository_revision");
    }
}
