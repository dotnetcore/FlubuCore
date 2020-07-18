using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class JenkinsPost
    {
        public JenkinsPostConditions Condition { get; set; }

        public List<string> Steps { get; set; }
    }
}
