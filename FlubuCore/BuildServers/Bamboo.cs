using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class Bamboo
    {
        public static bool IsRunningOnBamboo => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("bamboo_buildNumber"));
    }
}
