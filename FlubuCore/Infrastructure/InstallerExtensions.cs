using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services.AddLogging();

            services
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<ICommandExecutor, CommandExecutor>()
                .AddSingleton<ITaskContextSession, TaskContextSession>()
                .AddSingleton<TargetTree>()
                .AddSingleton<ITaskSession, TaskSession>()
                .AddSingleton<IComponentProvider, ComponentProvider>()
                .AddSingleton<ICommandFactory, CommandFactory>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>();

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