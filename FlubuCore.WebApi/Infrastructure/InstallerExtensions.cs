using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Context.FluentInterface.TaskExtensions.Core;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Processors;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.FlubuWebApi;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.WebApi.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCoreComponentsForWebApi(this IServiceCollection services)
        {
            services
                .AddLogging()
                .AddSingleton<ITimeProvider, TimeProvider>()
                .AddSingleton<IFluentInterfaceFactory, FluentInterfaceFactory>()
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IPathWrapper, PathWrapper>()
                .AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                .AddSingleton<IFlubuEnviromentService, FlubuEnviromentService>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>()
                .AddScoped<IBuildPropertiesSession, BuildPropertiesSession>()
                .AddScoped<TargetTree>()
                .AddScoped<ITaskSession, TaskSession>()
                .AddScoped<ICommandFactory, CommandFactory>()
                .AddScoped<CommandArguments, CommandArguments>();

            return services;
        }

        public static IServiceCollection AddCommandComponentsForWebApi(this IServiceCollection services)
        {
            services
                .AddScoped<IBuildScriptLocator, BuildScriptLocator>()
                .AddScoped<IScriptLoader, ScriptLoader>()
                .AddScoped<ICommandExecutor, CommandExecutor>();

            return services;
        }

        public static IServiceCollection AddScriptAnalyserForWebApi(this IServiceCollection services)
        {
            return services
                .AddScoped<IScriptAnalyser, ScriptAnalyser>()
                .AddScoped<IDirectiveProcessor, CsDirectiveProcessor>()
                .AddScoped<IDirectiveProcessor, ClassDirectiveProcessor>()
                .AddScoped<IDirectiveProcessor, AssemblyDirectiveProcessor>()
                .AddScoped<IDirectiveProcessor, ReferenceDirectiveProcessor>()
                .AddScoped<IDirectiveProcessor, NamespaceDirectiveProcessor>();
        }

        public static IServiceCollection AddTasksForWebApi(this IServiceCollection services)
        {
            return services
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
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
