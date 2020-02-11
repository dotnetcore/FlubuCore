using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNet.Cli.Flubu
{
    public static class Program
    {
        private static readonly IServiceCollection Services = new ServiceCollection();

        private static IServiceProvider _provider;

        private static ILogger<CommandExecutor> _logger;

        private static bool _cleanUpPerformed = false;

        private static volatile bool _wait = false;

        public static async Task<int> Main(string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }

            IServiceCollection startUpServiceCollection = new ServiceCollection();

            startUpServiceCollection.AddScriptAnalyzers()
                .AddCoreComponents()
                .AddCommandComponents(false)
                .AddScriptAnalyzers();

            Services
                .AddCommandComponentsWithArguments(args, startUpServiceCollection)
#if !NETCOREAPP1_0 && !NETCOREAPP1_1
              .AddFlubuLogging(startUpServiceCollection)
#else
                .AddFlubuLogging()
#endif
                .AddCoreComponents()
                .AddScriptAnalyzers()
                .AddTasks();

            var startupProvider = startUpServiceCollection.BuildServiceProvider();

            IScriptProvider scriptProvider = startupProvider.GetRequiredService<IScriptProvider>();
            CommandArguments commandArguments = startupProvider.GetRequiredService<CommandArguments>();
            ILoggerFactory loggerFactory = startupProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new FlubuLoggerProvider());
            _logger = startupProvider.GetRequiredService<ILogger<CommandExecutor>>();
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            _logger.LogInformation($"Flubu v.{version}");
            var script = await scriptProvider.GetBuildScriptAsync(commandArguments);
            Services.AddSingleton<ILoggerFactory>(loggerFactory);
            script.ConfigureServices(Services);
            _provider = Services.BuildServiceProvider();
            var cmdApp = _provider.GetRequiredService<CommandLineApplication>();

            ICommandExecutor executor = _provider.GetRequiredService<ICommandExecutor>();

            executor.FlubuHelpText = cmdApp.GetHelpText();
            Console.CancelKeyPress += OnCancelKeyPress;
            var result = await executor.ExecuteAsync();

            while (_wait)
            {
                Thread.Sleep(250);
            }

            return result;
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs eventArgs)
        {
            if (!_cleanUpPerformed && CleanUpStore.TaskCleanUpActions?.Count > 0)
            {
                _wait = true;
                _logger.LogInformation($"Performing clean up actions:");
                var taskSession = _provider.GetService<IFlubuSession>();
                foreach (var cleanUpAction in CleanUpStore.TaskCleanUpActions)
                {
                    cleanUpAction.Invoke(taskSession);
                    _logger.LogInformation($"Finished performing clean up actions.");
                }

                _wait = false;
            }
        }
    }
}
