using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
using FlubuCore.Extensions;
using FlubuCore.Scripting;
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

            Context = new TaskContext(loggerFactory.CreateLogger<TaskSession>(), new TaskContextSession(), new CommandArguments(), Factory, new CoreTaskFluentInterface(), new TaskFluentInterface());
        }

        protected ITaskFactory Factory { get; }

        protected ITaskContext Context { get; }

        protected IServiceProvider ServiceProvider { get; }
    }
}
