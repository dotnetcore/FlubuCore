using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.GitHubActions
{
    public class NameStep : IGitHubActionStep
    {
        public string Name { get; set; }

        [YamlMember(Alias = "working-directory", ApplyNamingConventions = false)]
        public string WorkingDirectory { get; set; }

        public string Uses { get; set; }

        public string Run { get; set; }

        internal NameStep Clone()
        {
            return (NameStep)MemberwiseClone();
        }
    }
}
