using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.AppVeyor
{
    public class Build
    {
        [YamlMember(Alias = "sh")]
        public string Shell { get; set; }

        [YamlMember(Alias = "ps")]
        public string PowerShell { get; set; }
    }
}
