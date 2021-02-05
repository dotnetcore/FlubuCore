using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlubuCore.BuildServers.Configurations
{
    public class GitHubActionsPushOptions
    {
        private readonly GitHubActionsOptions _options;

        public GitHubActionsPushOptions(GitHubActionsOptions options)
        {
            _options = options;
        }

        public GitHubActionsOptions AddBranchesToIgnoreOnPush(params string[] branches)
        {
            _options.BranchesToIgnoreOnPush = branches.ToList();
            return _options;
        }

        public GitHubActionsOptions AddBranchesOnPush(params string[] branches)
        {
            _options.BranchesOnPush = branches.ToList();
            return _options;
        }

        public GitHubActionsOptions AddTagsOnPush(params string[] tags)
        {
            _options.TagsOnPush = tags.ToList();
            return _options;
        }

        public GitHubActionsOptions AddTagsToIgnoreOnPush(params string[] tags)
        {
            _options.TagsToIgnoreOnPush = tags.ToList();
            return _options;
        }

        public GitHubActionsOptions AddPathsToIgnoreWhenPush(params string[] paths)
        {
            _options.PathsToIgnoreOnPush = paths.ToList();
            return _options;
        }

        public GitHubActionsOptions AddPathsWhenPush(params string[] paths)
        {
            _options.PathsOnPush = paths.ToList();
            return _options;
        }
    }
}
