using DotNet.Cli.Flubu.Commanding;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Analysis.Processors;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Cli.Flubu.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCommandComponentsWithArguments(this IServiceCollection services,
            string[] args)
        {
            var commandArguments = AddArgumentsImpl(services, args);

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
                .AddLogging()
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>()
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

        private static CommandArguments AddArgumentsImpl(IServiceCollection services, string[] args)
        {
            var app = new CommandLineApplication(false);
            IFlubuCommandParser parser = new FlubuCommandParser(app, new FlubuConfigurationProvider());

            services
                .AddSingleton(parser)
                .AddSingleton(app);

            if (args == null)
            {
                return null;
            }

            var commandArguments = parser.Parse(args);
            services.AddSingleton(commandArguments);
            return commandArguments;
        }
    }
}