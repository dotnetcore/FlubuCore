using System;
using System.Net.Http;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Analysis.Processors;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Docker;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.FlubuWebApi;
using FlubuCore.Tasks.Git;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;
using FlubuCore.Templating.Tasks;
using FlubuCore.WebApi.Client;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlubuCore.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services
                .AddSingleton<IFluentInterfaceFactory, FluentInterfaceFactory>()
                .AddSingleton<IScriptServiceProvider, ScriptServiceProvider>()
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IPathWrapper, PathWrapper>()
                .AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                .AddSingleton<IBuildPropertiesSession, BuildPropertiesSession>()
                .AddSingleton<TargetTree>()
                .AddSingleton<IFlubuSession, FlubuSession>()
                .AddSingleton<IFlubuEnvironmentService, FlubuEnvironmentService>()
                .AddSingleton<IBuildServer, BuildServer>()
                .AddSingleton<INugetPackageResolver, NugetPackageResolver>()
                .AddSingleton<ICommandFactory, CommandFactory>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .AddSingleton<IWebApiClientFactory, WebApiClientFactory>()
                .AddSingleton<IScriptProperties, ScriptProperties>()
                .AddSingleton<ITargetCreator, TargetCreator>()
                .AddSingleton<IFlubuTemplateTaskFactory, FlubuTemplateTaskFactory>()
                .AddSingleton<IFlubuTemplateTasksExecutor, FlubuTemplateTasksExecutor>()
                .AddTemplateTasks();

            return services;
        }

        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            return services
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddTransient<IToolsFluentInterface, ToolsFluentInterface>()
                .AddTransient<IGitFluentInterface, GitFluentInterface>()
                .AddTransient<DockerFluentInterface>()
                .AddTransient<ITarget, TargetFluentInterface>()
                .AddTransient<GenerateCommonAssemblyInfoTask>()
                .AddTransient<FetchBuildVersionFromFileTask>()
                .AddTransient<FetchVersionFromExternalSourceTask>()
                .AddTransient<CreateWebsiteTask>()
                .AddTransient<AddWebsiteBindingTask>()
                .AddTransient<OpenCoverTask>()
                .AddTask<LoadSolutionTask>()
                .AddTask<CompileSolutionTask>()
                .AddTask<CleanOutputTask>()
                .AddTask<DotnetRestoreTask>()
                .AddTask<DotnetTestTask>()
                .AddTask<DotnetBuildTask>()
                .AddTask<DotnetMsBuildTask>()
                .AddTask<DotnetPublishTask>()
                .AddTask<DotnetPackTask>()
                .AddTask<DotnetCleanTask>()
                .AddTask<DeletePackagesTask>()
                .AddTask<DeleteReportsTask>()
                .AddTask<GitAddTask>()
                .AddTask<GitCleanTask>()
                .AddTask<GitFetchTask>()
                .AddTask<GitPullTask>()
                .AddTask<GitCommitTask>()
                .AddTask<GitPushTask>()
                .AddTask<GitBranchTask>()
                .AddTask<GitMergeTask>()
                .AddTask<DockerStopTask>()
                .AddTask<T4TemplateTask>()
                .AddTask<TouchFileTask>()
                .AddTask<GitSubmoduleTask>()
                .AddTask<GitVersionTask>();
        }

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
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            var loggerFactory = ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>();
            var loggers = ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>));
            var loggerConfiguration = ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>((IConfigureOptions<LoggerFilterOptions>)new DefaultFlubuLoggerLevelConfigurationOptions(Microsoft.Extensions.Logging.LogLevel.Information));
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

        private static IServiceCollection AddTemplateTasks(this IServiceCollection services)
        {
            services.AddTransient<TemplateReplacementTokenTask>();
            return services;
        }
    }
}