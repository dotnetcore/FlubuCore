using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.Iis.Interfaces;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Cli.Flubu.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCommandComponents(this IServiceCollection services)
        {
            services
                .AddLogging()
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<ICommandExecutor, CommandExecutor>()
                .AddSingleton<ITaskFluentInterface, TaskFluentInterface>()
                .AddSingleton<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddSingleton<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddSingleton<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<GenerateCommonAssemblyInfoTask>()
                .AddTransient<FetchBuildVersionFromFileTask>()
                .AddTransient<FetchVersionFromExternalSourceTask>()
                .AddTransient<CreateWebsiteTask>()
                .AddTransient<CreateAppPoolTask>()
                .AddTransient<CreateWebApplicationTask>()
                .AddTransient<ControlAppPoolTask>()
                .AddTransient<DeleteAppPoolTask>()
                .AddTransient<AddWebsiteBindingTask>()
                .AddTransient<OpenCoverTask>();

            return services;
        }

        public static IServiceCollection AddArguments(this IServiceCollection services, string[] args)
        {
            var app = new CommandLineApplication();
            IFlubuCommandParser parser = new FlubuCommandParser(app);

            services
                .AddSingleton(parser)
                .AddSingleton(app)
                .AddSingleton(parser.Parse(args));

            return services;
        }
    }
}