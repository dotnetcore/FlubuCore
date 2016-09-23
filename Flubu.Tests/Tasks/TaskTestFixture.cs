using System;
using Microsoft.Extensions.Logging;

namespace Flubu.Tests.Tasks
{
    public class TaskTestFixture : IDisposable
    {
        public TaskTestFixture()
        {
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddConsole((s, l) => l >= LogLevel.Information);
        }

        public ILoggerFactory LoggerFactory { get; }

        public void Dispose()
        {
        }
    }
}
