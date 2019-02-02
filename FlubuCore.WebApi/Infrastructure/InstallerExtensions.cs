using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Analysis.Processors;
using FlubuCore.Services;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.FlubuWebApi;
using FlubuCore.Tasks.Git;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Repository;
using FlubuCore.WebApi.Services;
using LiteDB;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.WebApi.Infrastructure
{
    public static class InstallerExtensions
    {
        public static IServiceCollection AddCoreComponentsForWebApi(this IServiceCollection services,  IConfigurationRoot configuration)
        {
            services
                .AddLogging()
                .AddSingleton<ITimeProvider, TimeProvider>()
                .AddSingleton<IFluentInterfaceFactory, FluentInterfaceFactory>()
                .AddSingleton<ITaskSessionFactory, TaskSessionFactory>()
                .AddSingleton<IFileWrapper, FileWrapper>()
                .AddSingleton<IPathWrapper, PathWrapper>()
                .AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                .AddSingleton<IFlubuEnviromentService, FlubuEnviromentService>()
                .AddSingleton<ITaskFactory, DotnetTaskFactory>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .AddSingleton<IBuildSystem, BuildSystem>()
                .AddScoped<IBuildPropertiesSession, BuildPropertiesSession>()
                .AddScoped<TargetTree>()
                .AddScoped<ITaskSession, TaskSession>()
                .AddScoped<ICommandFactory, CommandFactory>()
                .AddScoped<CommandArguments, CommandArguments>();

            var connectionStrings = configuration.GetSection("FlubuConnectionStrings");
            var liteDbConnectionString = connectionStrings["LiteDbConnectionString"];

            var db = new LiteRepository(liteDbConnectionString);
            ILiteRepositoryFactory liteRepositoryFactory = new LiteRepositoryFactory();
            services.AddSingleton(liteRepositoryFactory);
            services.AddSingleton<IRepositoryFactory>(new RepositoryFactory(liteRepositoryFactory, new TimeProvider()));
            services.AddSingleton(db);
            services.AddScoped<ApiExceptionFilter>();
            services.AddScoped<ValidateRequestModelAttribute>();
            services.AddScoped<EmailNotificationFilter>();
            services.AddScoped<RestrictApiAccessFilter>();
            services.AddTransient<IHashService, HashService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ISecurityRepository, SecurityRepository>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INugetPackageResolver, NugetPackageResolver>();

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
                .AddScoped<IProjectFileAnalyzer, ProjectFileAnalyzer>()
                .AddScoped<IScriptAnalyzer, ScriptAnalyzer>()
                .AddScoped<IScriptProcessor, CsDirectiveProcessor>()
                .AddScoped<IScriptProcessor, ClassDirectiveProcessor>()
                .AddScoped<IScriptProcessor, AssemblyDirectiveProcessor>()
                .AddScoped<IScriptProcessor, ReferenceDirectiveProcessor>()
                .AddScoped<IScriptProcessor, NamespaceProcessor>()
                .AddScoped<IScriptProcessor, NugetPackageDirectirveProcessor>();
        }

        public static IServiceCollection AddTasksForWebApi(this IServiceCollection services)
        {
            return services
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
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
                .AddTask<DotnetPublishTask>()
                .AddTask<DotnetPackTask>()
                .AddTask<DotnetCleanTask>()
                .AddTask<DeletePackagesTask>()
                .AddTask<DeleteReportsTask>()
                .AddTask<GitAddTask>()
                .AddTask<GitPullTask>()
                .AddTask<GitCommitTask>()
                .AddTask<GitPushTask>()
                .AddTask<GitSubmoduleTask>()
                .AddTask<GitVersionTask>();
        }
    }
}
