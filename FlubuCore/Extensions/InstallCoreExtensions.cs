using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.IO.Wrappers;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Extensions
{
    public static class InstallCoreExtensions
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services
                .AddLogging()
                .AddSingleton<IFluentInterfaceFactory, FluentInterfaceFactory>()
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IBuildPropertiesSession, TaskContextSession>()
                .AddSingleton<TargetTree>()
                .AddSingleton<ITaskSession, TaskSession>()
                .AddSingleton<IFlubuEnviromentService, FlubuEnviromentService>()
                .AddSingleton<ICommandFactory, CommandFactory>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>();

            return services;
        }
    }
}
