using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FlubuCore.IO
{
    public static class IOExtensions
    {
        public static string GetFullPath(string path)
        {
            FileInfo info = new FileInfo(path);

            return info.FullName;
        }

        public static string GetUserProfileFolder(bool isWindows)
        {
            // todo fix when Environment is available https://github.com/dotnet/corefx/issues/5248
            if (isWindows)
                return Environment.GetEnvironmentVariable("USERPROFILE");

            return "~/";
        }

        public static string GetNodePath(bool isWindows)
        {
            // todo find node executable
            return isWindows ? "C:/Program Files/nodejs/node.exe" : "/usr/bin/node";
        }

        public static string GetNpmExecutablePath(bool isWindows)
        {
            // todo find node executable
            return isWindows ? "C:/Program Files/nodejs/npm.cmd" : "/usr/bin/npm";
        }

        public static string GetNpmPath(bool isWindows)
        {
            // todo find node executable
            var user = GetUserProfileFolder(isWindows);

            return Path.Combine(user, isWindows ? "AppData/Roaming/npm" : ".npm");
        }
    }
}
