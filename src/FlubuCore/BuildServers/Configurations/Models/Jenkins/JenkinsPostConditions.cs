using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public enum JenkinsPostConditions
    {
        Always,
        Changed,
        Fixed,
        Regression,
        Aborted,
        Failure,
        Success,
        Unstable,
        Unsuccesfull,
        Cleanup
    }
}
