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
    public class FlubuEngine : IFlubuEngine
    {
        public FlubuEngine()
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

            TaskFactory = new DotnetTaskFactory(ServiceProvider);
#endif
        }

        public FlubuEngine(IServiceCollection serviceCollection, ILoggerFactory loggerFactory = null)
        {
#if NETSTANDARD1_6
            throw new NotSupportedException("BuildScript engine is only supported in  =<.net standard2.0 and =<.net 4.62");
#endif
#if !NETSTANDARD1_6
            LoggerFactory = loggerFactory == null ? new LoggerFactory() : loggerFactory;

            ServiceProvider = serviceCollection.AddCoreComponents()
                .AddTasks()
                .BuildServiceProvider();

            TaskFactory = new DotnetTaskFactory(ServiceProvider);
#endif
        }

        public ITaskFactory TaskFactory { get; }

        public IServiceProvider ServiceProvider { get; }

        public ILoggerFactory LoggerFactory { get; }

        public static IServiceCollection AddFlubuComponents(IServiceCollection serviceCollection)
        {
            serviceCollection.AddCoreComponents().AddTasks();
            serviceCollection.AddSingleton<IFlubuEngine, FlubuEngine>();
            return serviceCollection;
        }

        public ITaskSession CreateTaskSession(BuildScriptArguments buildScriptArguments)
        {
            CommandArguments commandArguments = new CommandArguments(buildScriptArguments);

           return new TaskSession(
                LoggerFactory.CreateLogger<TaskSession>(),
                new TargetTree(ServiceProvider, commandArguments),
                commandArguments,
                new DotnetTaskFactory(ServiceProvider),
                new FluentInterfaceFactory(ServiceProvider),
                ServiceProvider.GetService<IBuildPropertiesSession>(),
                ServiceProvider.GetService<IBuildServers>());
        }
    }
}
