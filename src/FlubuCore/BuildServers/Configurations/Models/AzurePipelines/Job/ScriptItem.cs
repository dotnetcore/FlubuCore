using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
   public class ScriptItem : IStep
    {
        public string Script { get; set; }

        [YamlMember(Alias = "displayName", ApplyNamingConventions = false)]
        public string DisplayName { get; set; }

        [YamlMember(Alias = "workingDirectory", ApplyNamingConventions = false)]
        public string WorkingDirectory { get; set; }

        internal ScriptItem Clone()
        {
            return (ScriptItem)MemberwiseClone();
        }
    }
}
