using System.IO;
using System.Runtime.InteropServices;
using FlubuCore.IO;
using FlubuCore.Targeting;

namespace FlubuCore.Tasks.NetCore
{
    public static class Dotnet
    {
        public static ExecuteDotnetTask Pack(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Pack)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ExecuteDotnetTask Test(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Test)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
            
            //todo set xml outout for tests
        }

        public static ExecuteDotnetTask Run(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Run)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static string FindDotnetExecutable()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            return IOExtensions.GetFullPath(isWindows ? "C:/Program Files/dotnet/dotnet.exe" : "/usr/bin/dotnet");
        }
    }
}