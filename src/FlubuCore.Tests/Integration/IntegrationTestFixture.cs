using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        public IntegrationTestFixture()
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
                builder =>
                {
                    builder.AddFilter((level) => level >= LogLevel.Information)
                           .AddConsole();
                });

            var services = new ServiceCollection()
                .AddLogging()
                .AddCoreComponents()
                .AddScriptAnalyzers()
                .AddParserComponents()
                .AddTasks();
            var config = new FlubuConfiguration();
            services.AddSingleton(config);
            services.AddSingleton(new CommandArguments());

            ServiceProvider = services.BuildServiceProvider();
        }

        public ILoggerFactory LoggerFactory { get; }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            LoggerFactory.Dispose();
        }
    }
}