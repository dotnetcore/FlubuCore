using System;
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
#if NETSTANDARD1_6
            throw new NotSupportedException("BuildScript engine is only supported in  =<.net standard2.0 and =<.net 4.62");
#endif
#if !NETSTANDARD1_6
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddProvider(new FlubuLoggerProvider());
            var loggers = ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>));
            var app = new CommandLineApplication(false);
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
            ServiceProvider = serviceCollection.BuildServiceProvider();

            TaskFactory = new DotnetTaskFactory(ServiceProvider);
#endif
        }

        public FlubuEngine(IServiceCollection serviceCollection, ILoggerFactory loggerFactory = null)
        {
#if NETSTANDARD1_6
            throw new NotSupportedException("BuildScript engine is only supported in  =<.net standard2.0 and =<.net 4.62");
#endif
#if !NETSTANDARD1_6
            LoggerFactory = loggerFactory == null ? new LoggerFactory() : loggerFactory;
            LoggerFactory.AddProvider(new FlubuLoggerProvider());
            ServiceProvider = serviceCollection.AddCoreComponents()
                .AddTasks()
                .BuildServiceProvider();

            TaskFactory = new DotnetTaskFactory(ServiceProvider);
#endif
        }

        public ITaskFactory TaskFactory { get; }

        public IServiceProvider ServiceProvider { get; }

        public ILoggerFactory LoggerFactory { get; }

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
                ServiceProvider.GetService<IScriptFactory>(),
                new DotnetTaskFactory(ServiceProvider),
                new FluentInterfaceFactory(ServiceProvider),
                ServiceProvider.GetService<IBuildPropertiesSession>(),
                ServiceProvider.GetService<IBuildSystem>());
        }

        public int RunScript<T>(string[] args)
            where T : IBuildScript, new()
        {
            var parser = ServiceProvider.GetService<IFlubuCommandParser>();
            var cmdArgs = parser.Parse(args);
            var taskSession = CreateTaskSession(cmdArgs);
            taskSession.Args.FlubuFileLocation = cmdArgs.FlubuFileLocation;
            taskSession.Args.FlubuHelpText = cmdArgs.FlubuHelpText;
            taskSession.Args.AdditionalOptions = cmdArgs.AdditionalOptions;
            taskSession.Args.Debug = cmdArgs.Debug;
            taskSession.ScriptArgs = cmdArgs.ScriptArguments;
            var script = new T();
            taskSession.TargetTree.BuildScript = script;
            return script.Run(taskSession);
        }
    }
}
