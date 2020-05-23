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
            return await DotNet.Cli.Flubu.Program.Main(args);
        }
    }
}
