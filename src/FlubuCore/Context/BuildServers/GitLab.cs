using System;

namespace FlubuCore.Context.BuildServers
{
    public class GitLab
    {
        public static bool RunningOnGitLabCi => Environment.GetEnvironmentVariable("CI_SERVER")?.Equals("yes", StringComparison.OrdinalIgnoreCase) ?? false;

        /// <summary>
        /// Indicates whether build is running on GitLab Ci.
        /// </summary>
        public bool IsRunningOnGitLabCi => RunningOnGitLabCi;
    }
}
