using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Services;
using FlubuCore.Targeting;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Cli.Flubu.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection RegisterAll(this IServiceCollection services)
        {
            services.AddLogging();

            services
                .AddSingleton<IFileExistsService, FileExistsService>()
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<ICommandExecutor, CommandExecutor>()
                .AddSingleton<IFileLoader, FileLoader>()
                .AddSingleton<ITaskContextSession, TaskContextSession>()
                .AddSingleton<TargetTree>()
                .AddSingleton<ITaskSession, TaskSession>()
                .AddSingleton<IComponentProvider, ComponentProvider>()
                .AddSingleton<ICommandFactory, CommandFactory>();

            return services;
        }

        public static IServiceCollection AddArguments(this IServiceCollection services, string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();
            IFlubuCommandParser parser = new FlubuCommandParser(app);

            services
                .AddSingleton(parser)
                .AddSingleton(app)
                .AddSingleton(parser.Parse(args));

            return services;
        }
    }
}