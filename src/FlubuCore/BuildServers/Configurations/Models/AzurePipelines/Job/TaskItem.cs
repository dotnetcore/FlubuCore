using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
    public class TaskItem : IStep
    {
        public string Task { get; set; }

        [YamlMember(Alias = "displayName", ApplyNamingConventions = false)]
        public string DisplayName { get; set; }

        public TaskInputs Inputs { get; set; }

        [YamlMember(Alias = "workingDirectory", ApplyNamingConventions = false)]
        public string WorkingDirectory { get; set; }

        internal TaskItem Clone()
        {
            var item = (TaskItem)MemberwiseClone();
            item.Inputs = Inputs.Clone();
            return item;
        }
    }
}
