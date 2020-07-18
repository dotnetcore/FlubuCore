using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class Stage
    {
        public string Name { get; set; }

        public List<string> Steps { get; set; }

        public string WorkingDirectory { get; set; }
    }
}
