using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Infrastructure
{
    public class FlubuLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, FlubuConsoleLogger> _loggers =
            new ConcurrentDictionary<string, FlubuConsoleLogger>();

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, CreateLoggerImplementation);
        }

        public void Dispose()
        {
        }

        private FlubuConsoleLogger CreateLoggerImplementation(string name)
        {
            return new FlubuConsoleLogger(name);
        }
    }
}