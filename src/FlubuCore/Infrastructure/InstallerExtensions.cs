using System.Net.Http;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Docker;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.FlubuWebApi;
using FlubuCore.Tasks.Git;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;
using FlubuCore.Templating.Tasks;
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
                .AddSingleton<IScriptServiceProvider, ScriptServiceProvider>()
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IPathWrapper, PathWrapper>()
                .AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                .AddSingleton<IBuildPropertiesSession, BuildPropertiesSession>()
                .AddSingleton<TargetTree>()
                .AddSingleton<IFlubuSession, FlubuSession>()
                .AddSingleton<IFlubuEnvironmentService, FlubuEnvironmentService>()
                .AddSingleton<IBuildServer, BuildServer>()
                .AddSingleton<INugetPackageResolver, NugetPackageResolver>()
                .AddSingleton<ICommandFactory, CommandFactory>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .AddSingleton<IWebApiClientFactory, WebApiClientFactory>()
                .AddSingleton<IScriptProperties, ScriptProperties>()
                .AddSingleton<ITargetCreator, TargetCreator>()
                .AddSingleton<IFlubuTemplateTaskFactory, FlubuTemplateTaskFactory>()
                .AddSingleton<IFlubuTemplateTasksExecutor, FlubuTemplateTasksExecutor>()
                .AddTemplateTasks();

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
                .AddTransient<IToolsFluentInterface, ToolsFluentInterface>()
                .AddTransient<IGitFluentInterface, GitFluentInterface>()
                .AddTransient<DockerFluentInterface>()
                .AddTransient<ITarget, TargetFluentInterface>()
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
                .AddTask<DotnetMsBuildTask>()
                .AddTask<DotnetPublishTask>()
                .AddTask<DotnetPackTask>()
                .AddTask<DotnetCleanTask>()
                .AddTask<DeletePackagesTask>()
                .AddTask<DeleteReportsTask>()
                .AddTask<GitAddTask>()
                .AddTask<GitCleanTask>()
                .AddTask<GitFetchTask>()
                .AddTask<GitPullTask>()
                .AddTask<GitCommitTask>()
                .AddTask<GitPushTask>()
                .AddTask<GitBranchTask>()
                .AddTask<GitMergeTask>()
                .AddTask<DockerStopTask>()
                .AddTask<T4TemplateTask>()
                .AddTask<TouchFileTask>()
                .AddTask<GitSubmoduleTask>()
                .AddTask<GitVersionTask>();
        }

        private static IServiceCollection AddTemplateTasks(this IServiceCollection services)
        {
            services.AddTransient<TemplateReplacementTokenTask>();
            return services;
        }
    }
}