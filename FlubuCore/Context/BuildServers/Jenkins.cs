using System;

namespace FlubuCore.Context.BuildServers
{
    public class Jenkins
    {
        public static bool RunningOnJenkins => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_HOME"));

        /// <summary>
        /// Indicates whether build is running on Jenkins build server.
        /// </summary>
        public bool IsRunningOnJenkins => RunningOnJenkins;

        /// <summary>
        /// Gets the build number which is identical to build id.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("BUILD_NUMBER");

        public string Workspace => Environment.GetEnvironmentVariable("WORKSPACE");

        public string SvnRevisionId => Environment.GetEnvironmentVariable("SVN_REVISION");

        public string GitCommitId => Environment.GetEnvironmentVariable("GIT_COMMIT");

        public string GitBranch => Environment.GetEnvironmentVariable("GIT_BRANCH");
    }
}
