using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Extensions;
using FlubuCore.Runner.Commanding;
using FlubuCore.Runner.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Runner
{
    class Program
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
