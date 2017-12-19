using System;
using System.Runtime.InteropServices;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flubu.Tests
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

            Context = new TaskContextInternal(
                loggerFactory.CreateLogger<TaskSession>(),
                new BuildPropertiesSession(),
                new CommandArguments() { },
                new TargetTree(ServiceProvider, new CommandArguments()),
                Factory,
                new FluentInterfaceFactory(ServiceProvider));
        }

        protected ITaskFactory Factory { get; }

        protected ITaskContextInternal Context { get; set; }

        protected IServiceProvider ServiceProvider { get; }

        protected ILoggerFactory LoggerFactory { get; }

        public string PathToDotnetExecutable
        {
            get
            {
                return GetOsPlatform() == OSPlatform.Windows ? "C:\\Program Files\\dotnet\\dotnet.exe" : "/usr/bin/dotnet";
            }
        }

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
