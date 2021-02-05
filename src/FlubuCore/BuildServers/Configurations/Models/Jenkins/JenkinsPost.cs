using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class JenkinsPost
    {
        internal JenkinsPostConditions Condition { get; set; }

        internal List<string> Steps { get; set; } = new List<string>();
    }
}
