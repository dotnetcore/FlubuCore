using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class GoCD
    {
        public static bool IsRunningOnGoCD => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GO_SERVER_URL"));
    }
}
