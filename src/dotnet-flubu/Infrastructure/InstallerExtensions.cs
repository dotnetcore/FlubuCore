using System;
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
        public static IServiceCollection AddCommandComponents(this IServiceCollection services, bool addCommandExecutor = true, bool interactiveMode = false)
        {
            services
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<IScriptProvider, ScriptProvider>()
                .AddSingleton<IFlubuConfigurationProvider, FlubuConfigurationProvider>();

            if (!addCommandExecutor)
            {
                return services;
            }

            if (interactiveMode)
            {
                services.AddSingleton<ICommandExecutor, CommandExecutorInteractive>();
            }
            else
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
                .AddSingleton<IScriptProcessor, InterfaceDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, AssemblyDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, ReferenceDirectiveProcessor>()
                .AddSingleton<IScriptProcessor, NamespaceProcessor>()
                .AddSingleton<IScriptProcessor, NugetPackageDirectirveProcessor>()
                .AddSingleton<IScriptProcessor, AttributesProcessor>();
        }

        public static IServiceCollection AddParserComponents(this IServiceCollection services)
        {
            AddArgumentsImpl(services);
            return services;
        }

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

        private static void AddArgumentsImpl(IServiceCollection services, IServiceCollection services2 = null)
        {
            var app = new CommandLineApplication()
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            var flubuConfigurationProviderDescriptor =
                ServiceDescriptor.Singleton<IFlubuConfigurationProvider, FlubuConfigurationProvider>();

            services
                .AddSingleton(app)
                .AddSingleton<IFlubuCommandParser, FlubuCommandParser>()
                .AddSingleton<IFlubuCommandParserFactory, FlubuCommandParserFactory>()
                .TryAdd(flubuConfigurationProviderDescriptor);

            services2?.AddSingleton<IFlubuCommandParser, FlubuCommandParser>()
                .AddSingleton<IFlubuCommandParserFactory, FlubuCommandParserFactory>()
                .AddSingleton(app);
        }
    }
}