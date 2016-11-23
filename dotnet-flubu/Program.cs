using System;
using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Extensions;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNet.Cli.Flubu
{
    public static class Program
    {
        private static readonly IServiceCollection Services = new ServiceCollection();

        private static IServiceProvider _provider;

        public static int Main(string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }

            Services
                .AddCoreComponents()
                .AddCommandComponents()
                .AddArguments(args)
                .AddTasks();

            _provider = Services.BuildServiceProvider();
            ILoggerFactory factory = _provider.GetRequiredService<ILoggerFactory>();
            factory.AddProvider(new FlubuLoggerProvider());

            ICommandExecutor executor = _provider.GetRequiredService<ICommandExecutor>();
            return executor.ExecuteAsync().Result;
        }
    }
}