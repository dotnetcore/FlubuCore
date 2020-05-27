using System;
using FlubuCore.Infrastructure;

namespace FlubuCore.BuildServers
{
    public class TeamCity
    {
        public static bool RunningOnTeamCity => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));

        /// <summary>
        /// Indicates whether build is running on TeamCity build server.
        /// </summary>
        public bool IsRunningOnTeamCity => RunningOnTeamCity;

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("BUILD_NUMBER");

        public string ProjectName => Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME");

        public bool IsPullRequest
        {
            get
            {
                var gitBranch = Environment.GetEnvironmentVariable("Git_Branch");

                return !string.IsNullOrEmpty(gitBranch) && gitBranch.Contains("PULL-REQUEST", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
