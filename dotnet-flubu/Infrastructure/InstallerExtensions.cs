using DotNet.Cli.Flubu.Commanding;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Processors;
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
                .AddSingleton<IFlubuConfigurationProvider, FlubuConfigurationProvider>();

            return services;
        }

        public static IServiceCollection AddScriptAnalyser(this IServiceCollection services)
        {
            return services
                .AddSingleton<IScriptAnalyser, ScriptAnalyser>()
                .AddSingleton<IDirectiveProcessor, CsDirectiveProcessor>()
                .AddSingleton<IDirectiveProcessor, ClassDirectiveProcessor>()
                .AddSingleton<IDirectiveProcessor, AssemblyDirectiveProcessor>()
                .AddSingleton<IDirectiveProcessor, ReferenceDirectiveProcessor>()
                .AddSingleton<IDirectiveProcessor, NamespaceDirectiveProcessor>()
                .AddSingleton<IDirectiveProcessor, NugetPackageDirectirveProcessor>();
        }

        public static IServiceCollection AddArguments(this IServiceCollection services, string[] args)
        {
            var app = new CommandLineApplication(false);
            IFlubuCommandParser parser = new FlubuCommandParser(app, new FlubuConfigurationProvider());

            services
                .AddSingleton(parser)
                .AddSingleton(app)
                .AddSingleton(parser.Parse(args));

            return services;
        }
    }
}