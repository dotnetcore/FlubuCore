using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context;
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
                .AddSingleton<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<GenerateCommonAssemblyInfoTask>()
                .AddTransient<FetchBuildVersionFromFileTask>()
                .AddTransient<FetchVersionFromExternalSourceTask>();

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