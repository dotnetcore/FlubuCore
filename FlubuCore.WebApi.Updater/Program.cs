using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace FlubuCore.WebApi.Updater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread.Sleep(2000);
            
            string frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()
                ?.FrameworkName;
            bool isWindows = true;
            if (args.Length > 0)
            {
                var succeed = bool.TryParse(args[0], out isWindows);
                if (!succeed)
                {
                    Console.WriteLine("Failed to parse argument.");
                    isWindows = true;
                }
            }

            bool isNetCore = frameworkName.StartsWith(".NETCoreApp");
            string flubuPath, deployScript;
            if (isWindows)
            {
              
               flubuPath = Path.GetFullPath("WebApi/flubu.exe");
               if (isNetCore)
               {
                   deployScript = Path.GetFullPath("Updates/WebApi/DeploymentScript.cs");
               }
               else
               {
                   deployScript = Path.GetFullPath("WebApi/DeployScript.cs");
               }
            }
            else
            {
                flubuPath = Path.GetFullPath("Updates/WebApi/Deploy.bat");
                deployScript = Path.GetFullPath("Updates/WebApi/DeploymentScript.cs");
                var processRestore = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(flubuPath),
                    FileName = "dotnet restore",
                    Arguments = "Deploy.csproj",
                });

                processRestore.WaitForExit();
            }

            Console.WriteLine($"path: {deployScript}");
            Console.WriteLine($"flubu path: {flubuPath}");
            int retry = 0;
            while (retry < 10)
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(flubuPath),
                    FileName = isWindows ? flubuPath : "dotnet flubu",
                    Arguments = $"-s={deployScript}"
                });

                process.WaitForExit();
                int code = process.ExitCode;
                Console.WriteLine($"flubu exit code: {code}");
                if (code == 0)
                {
                    break;
                }

                retry++;
            }

            Thread.Sleep(2000);
        }
    }
}
