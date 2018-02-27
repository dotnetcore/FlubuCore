using System;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting
{
    public class BuildScriptEngine : IBuildScriptEngine
    {
        public BuildScriptEngine()
        {
#if NETSTANDARD1_6
            throw new NotSupportedException("BuildScript engine is only supported in  =<.net standard2.0 and =<.net 4.62");
#endif
#if !NETSTANDARD1_6
            LoggerFactory = new LoggerFactory();

            ServiceProvider = new ServiceCollection()
                .AddCoreComponents()
                .AddTasks()
                .BuildServiceProvider();

            Factory = new DotnetTaskFactory(ServiceProvider);
#endif
        }

        public BuildScriptEngine(IServiceCollection serviceCollection, ILoggerFactory loggerFactory = null)
        {
#if NETSTANDARD1_6
            throw new NotSupportedException("BuildScript engine is only supported in  =<.net standard2.0 and =<.net 4.62");
#endif
#if !NETSTANDARD1_6
            LoggerFactory = loggerFactory == null ? new LoggerFactory() : loggerFactory;

            ServiceProvider = serviceCollection.AddCoreComponents()
                .AddTasks()
                .BuildServiceProvider();

            Factory = new DotnetTaskFactory(ServiceProvider);
#endif
        }

        public ITaskFactory Factory { get; }

        public IServiceProvider ServiceProvider { get; }

        public ILoggerFactory LoggerFactory { get; }

        public ITaskSession CreateTaskSession(CommandArguments commandArguments)
        {
           return new TaskSession(
                LoggerFactory.CreateLogger<TaskSession>(),
                new TargetTree(ServiceProvider, commandArguments),
                commandArguments,
                new DotnetTaskFactory(ServiceProvider),
                new FluentInterfaceFactory(ServiceProvider),
                ServiceProvider.GetService<IBuildPropertiesSession>());
        }
    }
}
