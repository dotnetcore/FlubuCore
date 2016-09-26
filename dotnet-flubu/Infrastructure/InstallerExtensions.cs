using Flubu.Commanding;
using Flubu.Context;
using Flubu.Scripting;
using Flubu.Targeting;
using Flubu.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Flubu.Infrastructure
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
                .AddSingleton<ITaskSession, TaskSession>();

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