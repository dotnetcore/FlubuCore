using System;
using Flubu.Commanding;
using Flubu.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flubu
{
    public class Program
    {
        private static readonly IServiceCollection _Services = new ServiceCollection();

        private static IServiceProvider _provider;

        public static int Main(string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }

            _Services.RegisterAll();

            _provider = _Services.BuildServiceProvider();
            var factory = _provider.GetRequiredService<ILoggerFactory>();
            factory.AddConsole(LogLevel.Trace);

            var executor = _provider.GetRequiredService<ICommandExecutor>();
            return executor.Execute(args).Result;
        }
    }
}