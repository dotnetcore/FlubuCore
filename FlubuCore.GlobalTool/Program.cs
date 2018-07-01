using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Commanding;
using FlubuCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.GlobalTool
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return DotNet.Cli.Flubu.Program.Main(args);
        }
    }
}
