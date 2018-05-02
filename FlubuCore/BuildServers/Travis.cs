using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class Travis
    {
        public static bool IsRunningOnTravis => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TRAVIS"));
    }
}
