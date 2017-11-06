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

        public static string GetUserProfileFolder()
        {
#if NETSTANDARD2_0 || NET462
             return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#endif
            bool isWindows = IsWindows();
            if (isWindows)
                return Environment.GetEnvironmentVariable("USERPROFILE");

            return "~/";
        }

        public static string GetNodePath()
        {
            // todo find node executable
            return IsWindows() ? "C:/Program Files/nodejs/node.exe" : "/usr/bin/node";
        }

        public static string GetNpmExecutablePath(bool isWindows)
        {
            // todo find node executable
            return IsWindows() ? "C:/Program Files/nodejs/npm.cmd" : "/usr/bin/npm";
        }

        public static string GetNpmPath()
        {
            // todo find node executable
            var user = GetUserProfileFolder();
            return Path.Combine(user, IsWindows() ? "AppData/Roaming/npm" : ".npm");
        }

        private static bool IsWindows()
        {
           return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
