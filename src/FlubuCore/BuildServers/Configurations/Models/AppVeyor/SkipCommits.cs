using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.AppVeyor
{
    public class SkipCommits
    {
        public List<string> Files { get; set; }
    }
}
