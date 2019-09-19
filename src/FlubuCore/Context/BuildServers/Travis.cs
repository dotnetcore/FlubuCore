using System;

namespace FlubuCore.Context.BuildServers
{
    public class Travis
    {
        public static bool RunningOnTravis => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TRAVIS"));

        /// <summary>
        /// Indicates whether build is running on Travis build server.
        /// </summary>
        public bool IsRunningOnTravis => RunningOnTravis;

        /// <summary>
        /// Gets the build id.
        /// </summary>
        public string BuildId => Environment.GetEnvironmentVariable("TRAVIS_BUILD_ID");

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("TRAVIS_BUILD_NUMBER");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public string BuildWorkingFolder => Environment.GetEnvironmentVariable("TRAVIS_BUILD_DIR");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public string CommitId => Environment.GetEnvironmentVariable("TRAVIS_COMMIT");
    }
}
