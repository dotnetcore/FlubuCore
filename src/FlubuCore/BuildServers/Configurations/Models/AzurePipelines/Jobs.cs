using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines
{
    public class Jobs
    {
        public List<JobItem> Job { get; set; }
    }
}
