using flubu.Commanding;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using flubu.Infrastructure;

namespace flubu
{
    public class Program
    {
        private static IServiceCollection _services = new ServiceCollection();
        private static IServiceProvider _provider;

#pragma warning disable RECS0154 // Parameter is never used
        public static int Main(string[] args)
#pragma warning restore RECS0154 // Parameter is never used
        {
            _services.RegisterAll();

            _provider = _services.BuildServiceProvider();
            ILoggerFactory factory = _provider.GetRequiredService<ILoggerFactory>();
            factory.AddConsole();

            ICommandExecutor executor = _provider.GetRequiredService<ICommandExecutor>();
            return executor.Execute(args);
        }
    }
}