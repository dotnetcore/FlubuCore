using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class Jenkins
    {
        public static bool IsRunningOnJenkins => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_HOME"));
    }
}
