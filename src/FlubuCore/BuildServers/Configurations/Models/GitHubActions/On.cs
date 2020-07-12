using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.GitHubActions
{
    public class On
    {
        public Push Push { get; set; }

        public PullRequest PullRequest { get; set; }

        public Schedule Schedule { get; set; }
    }
}
