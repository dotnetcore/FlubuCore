using FlubuCore.Tasks.Solution;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Tasks
{
    public static class InstallTaskExtensions
    {
        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            services
                .AddTask<LoadSolutionTask>()
                .AddTask<CompileSolutionTask>();

            return services;
        }

        public static IServiceCollection AddTask<T>(this IServiceCollection services)
            where T : class
        {
            return services.AddSingleton<T, T>();
        }
    }
}
