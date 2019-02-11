using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FlubuCore.WebApi.Updater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread.Sleep(1000);
            var flubuPath = Path.GetFullPath("Updates/WebApi/flubu.exe");
            var deployScript = Path.GetFullPath("Updates/WebApi/DeployScript.cs");
            Console.WriteLine($"path: {deployScript}");
            var process = Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(flubuPath),
                FileName = flubuPath,
                Arguments = $"-s={deployScript}"
            });
         
            process.WaitForExit();
            int code = process.ExitCode;
            Console.WriteLine($"flubu exit code: {code}");
        }
    }
}
