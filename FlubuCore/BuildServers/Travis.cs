using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class Travis
    {
        public static bool IsRunningOnTravis => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TRAVIS"));

        /// <summary>
        /// Gets the build id.
        /// </summary>
        public static string BuildId => Environment.GetEnvironmentVariable("TRAVIS_BUILD_ID");

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public static string BuildNumber => Environment.GetEnvironmentVariable("TRAVIS_BUILD_NUMBER");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public static string BuildWorkingFolder => Environment.GetEnvironmentVariable("TRAVIS_BUILD_DIR");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public static string CommitId => Environment.GetEnvironmentVariable("TRAVIS_COMMIT");
    }
}
