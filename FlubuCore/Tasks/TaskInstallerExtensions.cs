using FlubuCore.Tasks.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Tasks
{
    public static class TaskInstallerExtensions
    {
        public static IServiceCollection AddTask<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : TaskBase, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddTransient<TaskBase, TImplementation>();
            return services;
        }

        public static IServiceCollection AddTask<TImplementation>(this IServiceCollection services)
            where TImplementation : TaskBase
        {
            services.AddTransient<TImplementation>();
            services.AddTransient<TaskBase, TImplementation>();
            return services;
        }

        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            return services.AddFileSystemTasks();
        }

        public static IServiceCollection AddFileSystemTasks(this IServiceCollection services)
        {
            return services
                .AddTask<CopyDirectoryStructureTask>()
                .AddTask<CopyFileTask>()
                .AddTask<CreateDirectoryTask>()
                .AddTask<DeleteDirectoryTask>()
                .AddTask<DeleteFilesTask>();
        }
    }
}
