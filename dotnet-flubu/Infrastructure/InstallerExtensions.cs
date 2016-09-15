using flubu.Scripting;
using Microsoft.Extensions.DependencyInjection;

namespace flubu.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection RegisterAll(this IServiceCollection services)
        {
            services.AddLogging();

            services
                .AddSingleton<IFileExistsService, FileExistsService>()
                .AddSingleton<IBuildScriptLocator, BuildScriptLocator>()
                .AddSingleton<IScriptLoader, ScriptLoader>();

            return services;
        }
    }
}
