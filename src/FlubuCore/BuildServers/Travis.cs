using System;
using FlubuCore.Services;

namespace FlubuCore.BuildServers
{
    public class Travis
    {
        public static bool RunningOnTravis => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TRAVIS"));

        /// <summary>
        /// Indicates whether build is running on Travis build server.
        /// </summary>
        public bool IsRunningOnTravis => RunningOnTravis;

        public string EventType => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_EVENT_TYPE");

        /// <summary>
        /// Build id of the current build.
        /// </summary>
        public string BuildId => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_BUILD_ID");

        public string RepoSlug => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_REPO_SLUG");

        /// <summary>
        /// Build number of the current build.
        /// </summary>
        public string BuildNumber => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_BUILD_NUMBER");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public string BuildWorkingFolder => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_BUILD_DIR");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public string CommitId => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_COMMIT");

        /// <summary>
        /// The commit message.
        /// </summary>
        public string CommitMessage => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_COMMIT_MESSAGE");

        /// <summary>
        /// Branch of the current build.
        /// </summary>
        public string BranchName => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_BRANCH");

        /// <summary>
        /// The tag name of the current build.
        /// </summary>
        public string TagName => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_TAG");

        /// <summary>
        /// The job identifier for the current job.
        /// </summary>
        public string JobId => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_JOB_ID");

        /// <summary>
        /// Job number for the current job.
        /// </summary>
        public string JobNumber => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_JOB_NUMBER");

        /// <summary>
        /// The Name of the operating system for the current job.
        /// </summary>
        public string OSName => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_OS_NAME");

        public bool IsPullRequest => Id > 0;

        /// <summary>
        /// Pull request id.
        /// </summary>
        public int Id => FlubuEnvironment.GetEnvironmentVariable<int>("TRAVIS_PULL_REQUEST");

        public string PullRequestBranch => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_PULL_REQUEST_BRANCH");

        /// <summary>
        /// The pull request commit id (hash) of the pull request.
        /// </summary>
        public string PullRequestSha => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_PULL_REQUEST_SHA");

        public string PullRequestSlug => FlubuEnvironment.GetEnvironmentVariable("TRAVIS_PULL_REQUEST_SLUG");
    }
}
