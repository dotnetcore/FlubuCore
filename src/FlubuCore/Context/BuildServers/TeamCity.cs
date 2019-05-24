using System;

namespace FlubuCore.Context.BuildServers
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
    }
}
