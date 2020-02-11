using System;
using System.Diagnostics;
using DotNet.Cli.Flubu.Commanding;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Analysis.Processors;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNet.Cli.Flubu.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCommandComponentsWithArguments(this IServiceCollection services,
            string[] args, IServiceCollection services2 = null)
        {
            var commandArguments = AddArgumentsImpl(services, args, services2);

            if (commandArguments == null || !commandArguments.InteractiveMode)
            {
                services.AddSingleton<ICommandExecutor, CommandExecutor>();
            }
            else
            {
                services.AddSingleton<ICommandExecutor, CommandExecutorInteractive>();
            }

            AddCommandComponents(services, false);

            return services;
        }

        public static IServiceCollection AddCommandComponents(this IServiceCollection services, bool addCommandExecutor = true)
        {
            services
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<IScriptProvider, ScriptProvider>()
                .AddSingleton<IFlubuConfigurationProvider, FlubuConfigurationProvider>();

            if (addCommandExecutor)
            {
                services.AddSingleton<ICommandExecutor, CommandExecutor>();
            }

            return services;
        }

        public static IServiceCollection AddScriptAnalyzers(this IServiceCollection services)
        {
            return services
                .AddSingleton<IProjectFileAnalyzer, ProjectFileAnalyzer>()
                .AddSingleton<IScriptAnalyzer, ScriptAnalyzer>()
                .AddSingleton<IScriptProcessor, CsDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, ClassDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, AssemblyDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, ReferenceDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, NamespaceProcessor>()
                .AddSingleton<IScriptProcessor, NugetPackageDirectirveProcessor>()
                .AddSingleton<IScriptProcessor, AttributesProcessor>();
        }

        public static IServiceCollection AddArguments(this IServiceCollection services, string[] args)
        {
            AddArgumentsImpl(services, args);
            return services;
        }

#if NETCOREAPP1_0 || NETCOREAPP1_1
    public static IServiceCollection AddFlubuLogging(this IServiceCollection services)
    {
      if (services == null)
        throw new ArgumentNullException(nameof(services));
      services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
      services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
      return services;
    }

#else
        public static IServiceCollection AddFlubuLogging(this IServiceCollection services, IServiceCollection services2 = null)
        {
            return services.AddFlubuLogging((Action<ILoggingBuilder>)(builder => { }), services2);
        }

        public static IServiceCollection AddFlubuLogging(
            this IServiceCollection services,
            Action<ILoggingBuilder> configure,
            IServiceCollection services2 = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddOptions();
            var loggerFactory = ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>();
            var loggers = ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>));
            var loggerConfiguration = ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>((IConfigureOptions<LoggerFilterOptions>)new DefaultFlubuLoggerLevelConfigurationOptions(LogLevel.Information));
           services.TryAdd(loggers);
            services.TryAddEnumerable(loggerConfiguration);
            var loggingBuilder = new LoggingBuilder(services);
            configure(loggingBuilder);

            if (services2 != null)
            {
                services2.AddOptions();
                services2.TryAdd(loggerFactory);
                services2.TryAdd(loggers);
                services2.TryAddEnumerable(loggerConfiguration);
                configure(loggingBuilder);
            }

            return services;
        }

#endif
        private static CommandArguments AddArgumentsImpl(IServiceCollection services, string[] args, IServiceCollection services2 = null)
        {
            var app = new CommandLineApplication(false);
            IFlubuCommandParser parser = new FlubuCommandParser(app, new FlubuConfigurationProvider());

            services
                .AddSingleton(parser)
                .AddSingleton(app);

            services2?.AddSingleton(parser)
                .AddSingleton(app);

            if (args == null)
            {
                return null;
            }

            var commandArguments = parser.Parse(args);

            services.AddSingleton(commandArguments);
            services2?.AddSingleton(commandArguments);
            return commandArguments;
        }
    }
}