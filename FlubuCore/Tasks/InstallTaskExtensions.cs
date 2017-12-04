using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Tasks
{
    public static class InstallTaskExtensions
    {
        public static IServiceCollection AddTask<T>(this IServiceCollection services)
            where T : class
        {
            return services.AddTransient<T, T>();
        }
    }
}
