using System;
using System.Threading.Tasks;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Commanding;
using FlubuCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.GlobalTool
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("This package is deprecated use FlubuCore.Tool instead. Package was just renamed to FlubuCore.Tool because as of .Net Core 3.0 local and global tools exists.");
            return await DotNet.Cli.Flubu.Program.Main(args);
        }
    }
}
