using flubu.Commanding;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using flubu.Infrastructure;

namespace flubu
{
    public class Program
    {
        private readonly static IServiceCollection _services = new ServiceCollection();
        private static IServiceProvider _provider;

        public static int Main(string[] args)
        {
            if (args == null)
                args = new string[0];

            _services.RegisterAll();

            _provider = _services.BuildServiceProvider();
            ILoggerFactory factory = _provider.GetRequiredService<ILoggerFactory>();
            factory.AddConsole();

            ICommandExecutor executor = _provider.GetRequiredService<ICommandExecutor>();
            return executor.Execute(args);
        }
    }
}