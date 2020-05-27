using System;
using System.IO;
using System.Runtime.InteropServices;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.NetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Tests
{
    public abstract class FlubuTestBase
    {
        protected FlubuTestBase(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;

            ServiceProvider = new ServiceCollection()
                .AddCoreComponents()
                .AddCommandComponents()
                .AddTasks()
                .BuildServiceProvider();

            Factory = new DotnetTaskFactory(ServiceProvider);
            var session = new BuildPropertiesSession(new TargetTree(ServiceProvider, new CommandArguments()));
            session.Set(BuildProps.ProductRootDir, Directory.GetCurrentDirectory());

            Context = new TaskContextInternal(
                loggerFactory.CreateLogger<FlubuSession>(),
                session,
                new CommandArguments() { },
                new TargetTree(ServiceProvider, new CommandArguments()),
                new BuildServer(),
                Factory,
                new FluentInterfaceFactory(ServiceProvider));
        }

        protected ITaskFactory Factory { get; }

        protected ITaskContextInternal Context { get; set; }

        protected IServiceProvider ServiceProvider { get; }

        protected ILoggerFactory LoggerFactory { get; }

        public string PathToDotnetExecutable => ExecuteDotnetTask.FindDotnetExecutable();

        public static OSPlatform GetOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            return OSPlatform.OSX;
        }
    }
}
