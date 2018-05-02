using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class ContinuaCl
    {
        public static bool IsRunningOnContinuaCl => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ContinuaCI.Version"));
    }
}
