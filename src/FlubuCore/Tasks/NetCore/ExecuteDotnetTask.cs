using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using FlubuCore.IO;

namespace FlubuCore.Tasks.NetCore
{
    public class ExecuteDotnetTask : ExecuteDotnetTaskBase<ExecuteDotnetTask>
    {
        private string _description;

        public ExecuteDotnetTask(string command)
            : base(command)
        {
        }

        public ExecuteDotnetTask(StandardDotnetCommands command)
            : base(command)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Execute dotnet command '{Command}'";
                }

                return _description;
            }

            set { _description = value; }
        }

        public static string FindDotnetExecutable()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            string dotnetExecutable = string.Empty;
            if (isWindows)
            {
                dotnetExecutable = "C:/Program Files/dotnet/dotnet.exe";
            }
            else if (isOsx)
            {
                dotnetExecutable = "/usr/local/share/dotnet/dotnet";
            }
            else if (isLinux)
            {
                dotnetExecutable = "/usr/bin/dotnet";
            }

            if (File.Exists(dotnetExecutable))
            {
                return dotnetExecutable;
            }

            return "dotnet";
        }
    }
}
