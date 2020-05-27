using System;

namespace FlubuCore.BuildServers
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

        /// <summary>
        /// The build tag which is a string of "jenkins-${JOB_NAME}-${BUILD_NUMBER}".
        /// </summary>
        public string BuildTag => Environment.GetEnvironmentVariable("BUILD_TAG");

        /// <summary>
        /// The name of the node this build is running on, such as "master".
        /// </summary>
        public string NodeName => Environment.GetEnvironmentVariable("NODE_NAME");

        /// <summary>
        /// The base name of the job.
        /// </summary>
        public string JobBaseName => Environment.GetEnvironmentVariable("JOB_BASE_NAME");

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        /// <value>
        /// The name of the job.
        /// </value>
        public string JobName => Environment.GetEnvironmentVariable("JOB_NAME");

        /// <summary>
        /// The Node labels.
        /// </summary>
        public string NodeLabels => Environment.GetEnvironmentVariable("NODE_LABELS");

        public string Workspace => Environment.GetEnvironmentVariable("WORKSPACE");

        public string SvnRevisionId => Environment.GetEnvironmentVariable("SVN_REVISION");

        /// <summary>
        /// Gets the Git commit id (hash).
        /// </summary>
        public string GitCommitId => Environment.GetEnvironmentVariable("GIT_COMMIT");

        /// <summary>
        /// Last successful commit id (hash).
        /// </summary>
        public string GitPreviousSuccessfulCommit => Environment.GetEnvironmentVariable("GIT_PREVIOUS_SUCCESSFUL_COMMIT");

        public string GitBranch => Environment.GetEnvironmentVariable("GIT_BRANCH");

        public string ChangeId => Environment.GetEnvironmentVariable("CHANGE_ID");

        public string ChangeAuthor => Environment.GetEnvironmentVariable("CHANGE_AUTHOR");

        public string ChangeAuthorEmail => Environment.GetEnvironmentVariable("CHANGE_AUTHOR_EMAIL");

        public string ChangeTarget => Environment.GetEnvironmentVariable("CHANGE_TARGET");

        public string BranchName => Environment.GetEnvironmentVariable("BRANCH_NAME");

        public string SvnUrl => Environment.GetEnvironmentVariable("SVN_URL");

        public string JenkinsUrl => Environment.GetEnvironmentVariable("JENKINS_URL");

        public bool IsPullRequest => !string.IsNullOrWhiteSpace(ChangeId);
    }
}
