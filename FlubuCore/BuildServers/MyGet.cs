using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class MyGet
    {
        public static bool IsRunningOnMyGet
        {
            get
            {
                var runner = Environment.GetEnvironmentVariable("BuildRunner");
                return !string.IsNullOrEmpty(runner) && string.Equals("MyGet", runner, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
