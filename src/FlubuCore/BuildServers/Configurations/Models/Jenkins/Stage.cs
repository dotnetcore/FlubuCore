using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class Stage
    {
        public string Name { get; set; }

        internal List<string> Steps { get; set; } = new List<string>();

        public string WorkingDirectory { get; set; }
    }
}
