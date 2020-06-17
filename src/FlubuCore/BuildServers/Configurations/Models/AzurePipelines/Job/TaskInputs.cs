using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
    public class TaskInputs
    {
        public string Version { get; set; }

        public string Command { get; set; }

        public string Custom { get; set; }

        public string Projects { get; set; }

        public string Arguments { get; set; }

        internal TaskInputs Clone()
        {
            return (TaskInputs)MemberwiseClone();
        }
    }
}
