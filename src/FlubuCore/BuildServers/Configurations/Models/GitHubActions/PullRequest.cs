using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.GitHubActions
{
    public class PullRequest
    {
        public List<string> Branches { get; set; }

        [YamlMember(Alias = "branches-ignore", ApplyNamingConventions = false)]
        public List<string> BranchesIgnore { get; set; }

        public List<string> Tags { get; set; }

        [YamlMember(Alias = "tags-ignore", ApplyNamingConventions = false)]
        public List<string> TagsIgnore { get; set; }

        [YamlMember(Alias = "paths-ignore", ApplyNamingConventions = false)]
        public List<string> PathsIgnore { get; set; }

        [YamlMember(Alias = "paths", ApplyNamingConventions = false)]
        public List<string> PathsInclude { get; set; }
    }
}
