using System;
using Flubu.Commanding;
using Flubu.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flubu
{
    public class Program
    {
        private static readonly IServiceCollection Services = new ServiceCollection();

        private static IServiceProvider provider;

        public static int Main(string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }

            Services.RegisterAll();

            provider = Services.BuildServiceProvider();
            var factory = provider.GetRequiredService<ILoggerFactory>();
            factory.AddConsole(LogLevel.Trace);

            var executor = provider.GetRequiredService<ICommandExecutor>();
            return executor.Execute(args).Result;
        }
    }
}