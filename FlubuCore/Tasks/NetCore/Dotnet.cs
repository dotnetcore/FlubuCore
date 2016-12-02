using System.IO;
using System.Runtime.InteropServices;
using FlubuCore.IO;
using FlubuCore.Targeting;

namespace FlubuCore.Tasks.NetCore
{
    public static class Dotnet
    {
        public static ITarget DotnetRestore(this ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(Dotnet.Restore(project));
            }

            return target;
        }

        public static ITarget DotnetBuild(this ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(Dotnet.Build(project));
            }

            return target;
        }

        public static ITarget DotnetPublish(this ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(Dotnet.Publish(project));
            }

            return target;
        }

        public static ExecuteDotnetTask Build(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Build)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ExecuteDotnetTask Restore(string projectName = null, string workingFolder = null)
        {
            ExecuteDotnetTask ret = new ExecuteDotnetTask(StandardDotnetCommands.Restore);

            if (!string.IsNullOrEmpty(workingFolder))
            {
                ret.WorkingFolder(workingFolder);
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                ret.WithArguments(projectName);
            }

            return ret;
        }

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
                .WithArguments(projectName)
                .XmlOutput($"{Path.GetFileNameWithoutExtension(projectName)}result.xml");
        }

        public static ExecuteDotnetTask Publish(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Publish)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName, "-c Release");
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