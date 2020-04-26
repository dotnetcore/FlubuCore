using System;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Tests
{
    public class FlubuTestFixture : IDisposable
    {
        public FlubuTestFixture()
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
                builder =>
                {
                    builder.AddFilter((level) => level >= LogLevel.Information)
                           .AddConsole();
                });
        }

        public ILoggerFactory LoggerFactory { get; }

        public void Dispose()
        {
            LoggerFactory.Dispose();
        }
    }
}
