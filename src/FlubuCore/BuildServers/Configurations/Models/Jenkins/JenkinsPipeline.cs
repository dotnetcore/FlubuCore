using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class JenkinsPipeline
    {
        public JenkinsOptionsDirective Options { get; set; }

        public List<Stage> Stages { get; set; }

        public List<JenkinsPost> Post { get; set; }
    }
}
