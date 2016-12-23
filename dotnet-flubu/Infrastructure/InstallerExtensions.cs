using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.Solution;
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
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<ICommandExecutor, CommandExecutor>();

            return services;
        }

        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            return services
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<ITargetFluentInterface, TargetFluentInterface>()
                .AddTransient<ITaskExtensionsFluentInterface, TaskExtensionsFluentInterface>()
                .AddTransient<GenerateCommonAssemblyInfoTask>()
                .AddTransient<FetchBuildVersionFromFileTask>()
                .AddTransient<FetchVersionFromExternalSourceTask>()
                .AddTransient<CreateWebsiteTask>()
                .AddTransient<CreateAppPoolTask>()
                .AddTransient<CreateWebApplicationTask>()
                .AddTransient<ControlAppPoolTask>()
                .AddTransient<DeleteAppPoolTask>()
                .AddTransient<AddWebsiteBindingTask>()
                .AddTransient<OpenCoverTask>()
                .AddTask<LoadSolutionTask>()
                .AddTask<CompileSolutionTask>()
                .AddTask<CleanOutputTask>();
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