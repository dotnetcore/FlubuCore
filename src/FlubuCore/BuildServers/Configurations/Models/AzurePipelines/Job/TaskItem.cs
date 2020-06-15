using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
    public class TaskItem : IStep
    {
        public string Task { get; set; }

        public string DisplayName { get; set; }

        public TaskInputs Inputs { get; set; }

        public string WorkingDirectory { get; set; }
    }
}
