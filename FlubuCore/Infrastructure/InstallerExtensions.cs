using System.Net.Http;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Context.FluentInterface.TaskExtensions.Core;
using FlubuCore.IO.Wrappers;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.FlubuWebApi;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;
using FlubuCore.WebApi.Client;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services
                .AddSingleton<IFluentInterfaceFactory, FluentInterfaceFactory>()
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IPathWrapper, PathWrapper>()
                .AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                .AddSingleton<IBuildPropertiesSession, BuildPropertiesSession>()
                .AddSingleton<TargetTree>()
                .AddSingleton<ITaskSession, TaskSession>()
                .AddSingleton<IFlubuEnviromentService, FlubuEnviromentService>()
                .AddSingleton<IBuildServers, Context.BuildServers>()
                .AddSingleton<ICommandFactory, CommandFactory>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .AddSingleton<IWebApiClientFactory, WebApiClientFactory>();

            return services;
        }

        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            return services
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddTransient<ITargetFluentInterface, TargetFluentInterface>()
                .AddTransient<ITaskExtensionsFluentInterface, TaskExtensionsFluentInterface>()
                .AddTransient<ICoreTaskExtensionsFluentInterface, CoreTaskExtensionsFluentInterface>()
                .AddTransient<GenerateCommonAssemblyInfoTask>()
                .AddTransient<FetchBuildVersionFromFileTask>()
                .AddTransient<FetchVersionFromExternalSourceTask>()
                .AddTransient<CreateWebsiteTask>()
                .AddTransient<AddWebsiteBindingTask>()
                .AddTransient<OpenCoverTask>()
                .AddTask<LoadSolutionTask>()
                .AddTask<CompileSolutionTask>()
                .AddTask<CleanOutputTask>()
                .AddTask<DotnetRestoreTask>()
                .AddTask<DotnetTestTask>()
                .AddTask<DotnetBuildTask>()
                .AddTask<DotnetPublishTask>()
                .AddTask<DotnetPackTask>()
                .AddTask<DotnetCleanTask>()
                .AddTask<DeletePackagesTask>();
        }
    }
}