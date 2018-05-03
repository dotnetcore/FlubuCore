using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class ContinuaCl
    {
        public static bool IsRunningOnContinuaCl => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ContinuaCI.Version"));

        public static string BuildNumber => Environment.GetEnvironmentVariable("ContinuaCI.BuildNumber");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public static string CommitId => Environment.GetEnvironmentVariable("ContinuaCI_Revision");
    }
}
