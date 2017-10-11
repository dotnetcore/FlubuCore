using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace FlubuCore.Services
{
    public static class FlubuEnviroment
    {
        /// <summary>
        /// Gets the Windows system root directory path.
        /// </summary>
        /// <value>The Windows system root directory path.</value>
        public static string SystemRootDir
        {
            get { return Environment.GetEnvironmentVariable("SystemRoot"); }
        }

        public static void FillVersionsFromMSBuildToolsVersionsRegPath(
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

                        object msBuildToolsPathObj = toolsVersionKey.GetValue("MSBuildToolsPath");
                        string msBuildToolsPath = msBuildToolsPathObj as string;
                        if (msBuildToolsPath != null) toolsVersions.Add(new Version(toolsVersion), msBuildToolsPath);
                    }
                }
            }
        }

        public static void FillVersion15FromVisualStudio2017RegPath(SortedDictionary<Version, string> toolsVersions)
        {
            using (RegistryKey vs2017Key =
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7", false))
            {
                if (vs2017Key == null)
                    return;

                object key150Value = vs2017Key.GetValue("15.0");
                string vs2017RootPath = key150Value as string;
                if (vs2017RootPath == null)
                    return;

                string msBuildToolsPath = Path.Combine(vs2017RootPath, @"MSBuild\15.0\Bin");
                toolsVersions.Add(new Version("15.0"), msBuildToolsPath);
            }
        }
    }
}
