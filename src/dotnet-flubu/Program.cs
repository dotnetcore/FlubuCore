using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore;
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

            var statusCode = await FlubuStartup(args);

            if (statusCode != 0)
            {
                return statusCode;
            }

            ICommandExecutor executor = _provider.GetRequiredService<ICommandExecutor>();

            Console.CancelKeyPress += OnCancelKeyPress;
            var result = await executor.ExecuteAsync();

            while (_wait)
            {
                Thread.Sleep(250);
            }

            return result;
        }

        private static async Task<int> FlubuStartup(string[] args)
        {
            IServiceCollection startUpServiceCollection = new ServiceCollection();

            startUpServiceCollection.AddScriptAnalyzers()
                .AddCoreComponents()
                .AddCommandComponents(false)
                .AddParserComponents()
                .AddScriptAnalyzers();

            Services.AddFlubuLogging(startUpServiceCollection);

            var startupProvider = startUpServiceCollection.BuildServiceProvider();
            var parser = startupProvider.GetRequiredService<IFlubuCommandParser>();
            var commandArguments = parser.Parse(args);
            IScriptProvider scriptProvider = startupProvider.GetRequiredService<IScriptProvider>();
            IFlubuConfigurationBuilder flubuConfigurationBuilder = new FlubuConfigurationBuilder();
            ILoggerFactory loggerFactory = startupProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new FlubuLoggerProvider());
            _logger = startupProvider.GetRequiredService<ILogger<CommandExecutor>>();
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            _logger.LogInformation($"Flubu v.{version}");

            IBuildScript script = null;
            if (!commandArguments.IsInternalCommand())
            {
                try
                {
                    script = await scriptProvider.GetBuildScriptAsync(commandArguments);
                }
                catch (BuildScriptLocatorException e)
                {
                    if (!commandArguments.InteractiveMode)
                    {
                        var str = commandArguments.Debug ? e.ToString() : e.Message;
                        _logger.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                        return StatusCodes.BuildScriptNotFound;
                    }
                }
            }

            Services
                .AddCoreComponents()
                .AddParserComponents()
                .AddCommandComponents(interactiveMode: commandArguments.InteractiveMode)
                .AddScriptAnalyzers()
                .AddTasks();

            Services.AddSingleton(loggerFactory);
            Services.AddSingleton(commandArguments);

            if (script != null)
            {
                script.ConfigureServices(Services);
                script.Configure(flubuConfigurationBuilder, loggerFactory);
            }

            Services.AddSingleton(flubuConfigurationBuilder.Build());

            _provider = Services.BuildServiceProvider();
            return 0;
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
