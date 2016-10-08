using System;
using System.Collections.Generic;
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

        public static IDictionary<Version, string> ListAvailableMSBuildToolsVersions()
        {
            SortedDictionary<Version, string> toolsVersions = new SortedDictionary<Version, string>();
            using (RegistryKey toolsVersionsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions", false))
            {
                if (toolsVersionsKey == null)
                    return toolsVersions;

                foreach (string toolsVersion in toolsVersionsKey.GetSubKeyNames())
                {
                    using (RegistryKey toolsVersionKey = toolsVersionsKey.OpenSubKey(toolsVersion, false))
                    {
                        if (toolsVersionKey == null)
                            continue;

                        object buildToolPathObj = toolsVersionKey.GetValue("MSBuildToolsPath");
                        string buildToolsPath = buildToolPathObj as string;
                        if (buildToolsPath != null)
                            toolsVersions.Add(new Version(toolsVersion), buildToolsPath);
                    }
                }
            }

            return toolsVersions;
        }
    }
}
