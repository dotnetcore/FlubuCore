using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class Bamboo
    {
        public static bool IsRunningOnBamboo => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("bamboo_buildNumber"));

        /// <summary>
        /// Gets the build id.
        /// </summary>
        public static string BuildKey => Environment.GetEnvironmentVariable("bamboo_buildKey");

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public static string BuildNumber => Environment.GetEnvironmentVariable("bamboo_buildNumber");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public static string BuildWorkingFolder => Environment.GetEnvironmentVariable("bamboo_build_working_directory");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public static string CommitId => Environment.GetEnvironmentVariable("bamboo_planRepository_revision");
    }
}
