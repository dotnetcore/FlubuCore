using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class BitRise
    {
        public static bool IsRunningOnBitrise => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BITRISE_BUILD_URL"));
    }
}
