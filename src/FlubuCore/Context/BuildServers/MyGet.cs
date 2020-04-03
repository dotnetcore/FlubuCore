using System;

namespace FlubuCore.Context.BuildServers
{
    public class MyGet
    {
        public static bool RunningOnMyGet
        {
            get
            {
                var runner = Environment.GetEnvironmentVariable("BuildRunner");
                return !string.IsNullOrEmpty(runner) && string.Equals("MyGet", runner, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Indicates whether build is running on MyGet.
        /// </summary>
        public bool IsRunningOnMyGet => RunningOnMyGet;
    }
}
