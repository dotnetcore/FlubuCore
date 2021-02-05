using System;

namespace FlubuCore.BuildServers
{
    public class GitHubActions
    {
        public static bool RunningOnGitHubActions => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

        /// <summary>
        /// Indicates whether build is running on Travis build server.
        /// </summary>
        public bool IsRunningOnGitHubActions => RunningOnGitHubActions;

        /// <summary>
        /// The path to the GitHub home directory used to store user data.
        /// </summary>
        public string Home => Environment.GetEnvironmentVariable("HOME");

        /// <summary>
        /// The name of the workflow.
        /// </summary>
        public string GitHubWorkflow => Environment.GetEnvironmentVariable("GITHUB_WORKFLOW");

        /// <summary>
        /// The name of the action.
        /// </summary>
        public string GitHubAction => Environment.GetEnvironmentVariable("GITHUB_ACTION");

        /// <summary>
        /// The name of the person or app that initiated the workflow.
        /// </summary>
        public string GitHubActor => Environment.GetEnvironmentVariable("GITHUB_ACTOR");

        /// <summary>
        /// The owner and repository name.
        /// </summary>
        public string GitHubRepository => Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");

        /// <summary>
        /// The GitHub workspace directory path. The workspace directory contains a subdirectory with a copy of your repository if your workflow uses the actions/checkout action. If you don't use the actions/checkout action, the directory will be empty.
        /// For example. <code>/home/runner/work/my-repo-name/my-repo-name.</code>
        /// </summary>
        public string GitHubWorkspace => Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");

        /// <summary>
        /// The name of the webhook event that triggered the workflow.
        /// </summary>
        public string GitHubEventName => Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");

        /// <summary>
        /// The path of the file with the complete webhook event payload.
        /// </summary>
        public string GitHubEventPath => Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH");

        /// <summary>
        /// The commit id (SHA) that triggered the workflow.
        /// </summary>
        public string GitHubCommitSha => Environment.GetEnvironmentVariable("GITHUB_SHA");

        /// <summary>
        /// The branch or tag ref that triggered the workflow.
        /// </summary>
        public string GitHubBranchRef => Environment.GetEnvironmentVariable("GITHUB_REF");

        /// <summary>
        /// Only set for forked repositories. The branch of the head repository.
        /// </summary>
        public string GitHubHeadRef => Environment.GetEnvironmentVariable("GITHUB_HEAD_REF");

        /// <summary>
        /// Only set for forked repositories. The branch of the base repository.
        /// </summary>
        public string GitHubBaseRef => Environment.GetEnvironmentVariable("GITHUB_BASE_REF");

        /// <summary>
        /// The git hub token.
        /// </summary>
        public string GitHubToken => Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    }
}
