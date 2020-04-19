using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Win32;

namespace FlubuCore.Services
{
    public static class FlubuEnvironment
    {
        private static List<string> _vsEditions = new List<string>()
        {
            "Preview",
            "BuildTools",
            "Professional",
            "Community",
            "Enterprise",
        };

        /// <summary>
        /// Gets the Windows system root directory path.
        /// </summary>
        /// <value>The Windows system root directory path.</value>
        public static string SystemRootDir => Environment.GetEnvironmentVariable("SystemRoot");

        public static string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        public static T GetEnvironmentVariable<T>(string variable)
        {
            string value = Environment.GetEnvironmentVariable(variable);

            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        public static void FillVersionsFromMsBuildToolsVersionsRegPath(
            SortedDictionary<Version, string> toolsVersions)
        {
            using (RegistryKey toolsVersionsKey =
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions", false))
            {
                if (toolsVersionsKey == null)
                    return;

                foreach (string toolsVersion in toolsVersionsKey.GetSubKeyNames())
                {
                    using (RegistryKey toolsVersionKey = toolsVersionsKey.OpenSubKey(toolsVersion, false))
                    {
                        if (toolsVersionKey == null) continue;

                        object buildToolsPathObj = toolsVersionKey.GetValue("MSBuildToolsPath");
                        if (buildToolsPathObj is string buildToolsPath) toolsVersions.Add(new Version(toolsVersion), buildToolsPath);
                    }
                }
            }
        }

        public static void FillVersion15FromVisualStudio2017(SortedDictionary<Version, string> toolsVersions)
        {
            using (RegistryKey vs2017Key =
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7", false))
            {
                if (vs2017Key == null)
                {
                    FillMsBuild15Path(toolsVersions);
                    return;
                }

                object key150Value = vs2017Key.GetValue("15.0");
                if (!(key150Value is string vs2017RootPath))
                {
                    FillMsBuild15Path(toolsVersions);
                    return;
                }

                string buildToolsPath = Path.Combine(vs2017RootPath, @"MSBuild\15.0\Bin");
                toolsVersions.Add(new Version("15.0"), buildToolsPath);
            }
        }

        internal static void FillMsBuild15Path(SortedDictionary<Version, string> toolsVersions)
        {
            string programFilesX86DirPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            foreach (var vsEdition in _vsEditions)
            {
                var msbuildPath = Path.Combine(programFilesX86DirPath, "Microsoft Visual Studio/2017", vsEdition, "MSBuild/15.0/Bin");

                if (Directory.Exists(msbuildPath))
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        msbuildPath = Path.Combine(msbuildPath, "amd64");
                    }

                    toolsVersions.Add(new Version("15.0"), msbuildPath);
                    return;
                }
            }
        }

        internal static void FillMsBuild16Path(SortedDictionary<Version, string> toolsVersions)
        {
            string programFilesX86DirPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            foreach (var vsEdition in _vsEditions)
            {
                var msbuildPath = Path.Combine(programFilesX86DirPath, "Microsoft Visual Studio/2019", vsEdition, "MSBuild/Current/Bin");

                if (Directory.Exists(msbuildPath))
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        msbuildPath = Path.Combine(msbuildPath, "amd64");
                    }

                    toolsVersions.Add(new Version("16.0"), msbuildPath);
                    return;
                }
            }
        }
    }
}
