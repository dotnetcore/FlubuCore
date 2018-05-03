using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class Jenkins
    {
        public static bool IsRunningOnJenkins => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_HOME"));

        /// <summary>
        /// Gets the build number which is identical to build id.
        /// </summary>
        public static string BuildNumber => Environment.GetEnvironmentVariable("BUILD_NUMBER");

        public static string Workspace => Environment.GetEnvironmentVariable("WORKSPACE");

        public static string SvnRevisionId => Environment.GetEnvironmentVariable("SVN_REVISION");

        public static string GitCommitId => Environment.GetEnvironmentVariable("GIT_COMMIT");
    }
}
