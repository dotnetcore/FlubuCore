using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Extensions;
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
            ServiceProvider = new ServiceCollection()
                .AddCoreComponents()
                .AddCommandComponents()
                .AddTasks()
                .BuildServiceProvider();

            Factory = new DotnetTaskFactory(ServiceProvider);

            Context = new TaskContextInternal(
                loggerFactory.CreateLogger<TaskSession>(),
                new TaskContextSession(),
                new CommandArguments(),
                new TargetTree(ServiceProvider, Factory),
                Factory,
                new CoreTaskFluentInterface(new LinuxTaskFluentInterface()),
                new TaskFluentInterface(new IisTaskFluentInterface()),
                new TargetFluentInterface());
        }

        protected ITaskFactory Factory { get; }

        protected ITaskContextInternal Context { get; }

        protected IServiceProvider ServiceProvider { get; }
    }
}
