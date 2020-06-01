using System;
using FlubuCore.BuildServers.Configurations;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace FlubuCore.Scripting
{
    public class FlubuEngine : IFlubuEngine
    {
        public FlubuEngine()
        {
            LoggerFactory = new LoggerFactory();
            FlubuConfigurationBuilder = new FlubuConfigurationBuilder();
            LoggerFactory.AddProvider(new FlubuLoggerProvider());
            var loggers = ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>));
            var app = new CommandLineApplication()
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };

            FlubuConfiguration flubuConfiguration = new FlubuConfiguration();
            CommandArguments arguments = new CommandArguments();
            var serviceCollection = new ServiceCollection()
                .AddCoreComponents()
                .AddTasks()
                .AddSingleton(app)
                .AddSingleton(LoggerFactory)
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IFlubuConfigurationProvider, FlubuConfigurationProvider>()
                .AddSingleton<IFlubuCommandParser, FlubuCommandParser>()
                .AddSingleton(arguments);

            serviceCollection.TryAdd(loggers);
            serviceCollection.AddSingleton(flubuConfiguration);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            TaskFactory = new DotnetTaskFactory(ServiceProvider);
        }

        public FlubuEngine(IServiceCollection serviceCollection, ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory == null ? new LoggerFactory() : loggerFactory;
            LoggerFactory.AddProvider(new FlubuLoggerProvider());
            ServiceProvider = serviceCollection.AddCoreComponents()
                .AddTasks()
                .BuildServiceProvider();

            TaskFactory = new DotnetTaskFactory(ServiceProvider);
            FlubuConfigurationBuilder = new FlubuConfigurationBuilder();
        }

        public ITaskFactory TaskFactory { get; }

        public IServiceProvider ServiceProvider { get; }

        public ILoggerFactory LoggerFactory { get; }

        public FlubuConfigurationBuilder FlubuConfigurationBuilder { get; }

        public static IServiceCollection AddFlubuComponents(IServiceCollection serviceCollection)
        {
            serviceCollection.AddCoreComponents().AddTasks();
            serviceCollection.AddSingleton<IFlubuEngine, FlubuEngine>();
            return serviceCollection;
        }

        public IFlubuSession CreateTaskSession(BuildScriptArguments buildScriptArguments)
        {
            CommandArguments commandArguments = new CommandArguments(buildScriptArguments);

            return new FlubuSession(
                LoggerFactory.CreateLogger<FlubuSession>(),
                ServiceProvider.GetService<TargetTree>(),
                commandArguments,
                ServiceProvider.GetService<IScriptServiceProvider>(),
                new DotnetTaskFactory(ServiceProvider),
                new FluentInterfaceFactory(ServiceProvider),
                ServiceProvider.GetService<IBuildPropertiesSession>(),
                ServiceProvider.GetService<IBuildServer>());
        }

        public int RunScript<T>(string[] args)
            where T : IBuildScript, new()
        {
            var parser = ServiceProvider.GetService<IFlubuCommandParser>();
            var flubuConfiguration = ServiceProvider.GetService<FlubuConfiguration>();

            var cmdArgs = parser.Parse(args);
            var taskSession = CreateTaskSession(cmdArgs);
            taskSession.Args.FlubuFileLocation = cmdArgs.FlubuFileLocation;
            taskSession.Args.FlubuHelpText = cmdArgs.FlubuHelpText;
            taskSession.Args.AdditionalOptions = cmdArgs.AdditionalOptions;
            taskSession.Args.Debug = cmdArgs.Debug;
            taskSession.Args.GenerateContinousIntegrationConfigs = cmdArgs.GenerateContinousIntegrationConfigs;
            taskSession.ScriptArgs = cmdArgs.ScriptArguments;
            var script = new T();
            taskSession.TargetTree.BuildScript = script;

            script.Configure(FlubuConfigurationBuilder, LoggerFactory);
            flubuConfiguration.CopyFrom(FlubuConfigurationBuilder);
            return script.Run(taskSession);
        }
    }
}
