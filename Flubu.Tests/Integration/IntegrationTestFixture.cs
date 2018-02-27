using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flubu.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        public IntegrationTestFixture()
        {
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddConsole((s, l) => l >= LogLevel.Information);
            var services = new ServiceCollection()
                .AddCoreComponents()
                .AddScriptAnalyser()
                .AddArguments(new[] { "flubu" })
                .AddTasks();

            ServiceProvider = services.BuildServiceProvider();
        }

        public ILoggerFactory LoggerFactory { get; }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
        }
    }
}
