using System.Linq;

namespace FlubuCore.BuildServers.Configurations
{
    public class GitHubActionsPullRequestOptions
    {
        private readonly GitHubActionsOptions _options;

        public GitHubActionsPullRequestOptions(GitHubActionsOptions options)
        {
            _options = options;
        }

        public GitHubActionsOptions AddBranchesToIgnore(params string[] branches)
        {
            _options.BranchesToIgnoreOnPullRequest = branches.ToList();
            return _options;
        }

        public GitHubActionsOptions AddBranches(params string[] branches)
        {
            _options.BranchesOnPullRequest = branches.ToList();
            return _options;
        }

        public GitHubActionsOptions AddTags(params string[] tags)
        {
            _options.TagsOnPullRequest = tags.ToList();
            return _options;
        }

        public GitHubActionsOptions AddTagsToIgnore(params string[] tags)
        {
            _options.TagsToIgnoreOnPullRequest = tags.ToList();
            return _options;
        }

        public GitHubActionsOptions AddPathsToIgnore(params string[] paths)
        {
            _options.PathsToIgnoreOnPullRequest = paths.ToList();
            return _options;
        }

        public GitHubActionsOptions AddPaths(params string[] paths)
        {
            _options.PathsOnPullRequest = paths.ToList();
            return _options;
        }
    }
}
