using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
   public class ScriptItem : IStep
    {
        public string Script { get; set; }

        public string DisplayName { get; set; }

        public string WorkingDirectory { get; set; }
    }
}
