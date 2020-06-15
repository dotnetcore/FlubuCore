using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
    public interface IStep
    {
        string WorkingDirectory { get; set; }
    }
}
