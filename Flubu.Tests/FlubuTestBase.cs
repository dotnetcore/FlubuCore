using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Context;
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
                .BuildServiceProvider();

            Context = new TaskContext(loggerFactory.CreateLogger<TaskSession>(), new TaskContextSession(), new CommandArguments(), new DotnetTaskFactory(ServiceProvider));
        }

        protected ITaskContext Context { get; set; }

        protected IServiceProvider ServiceProvider { get; set; }
    }
}
