using System;

namespace FlubuCore.BuildServers
{
    public class ContinuaCl
    {
        public static bool RunningOnContinuaCl => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ContinuaCI.Version"));

        /// <summary>
        /// Indicates whether build is running on ContinuaCL build server.
        /// </summary>
        public bool IsRunningOnContinuaCl => RunningOnContinuaCl;

        public string BuildNumber => Environment.GetEnvironmentVariable("ContinuaCI.BuildNumber");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public string CommitId => Environment.GetEnvironmentVariable("ContinuaCI_Revision");
    }
}
