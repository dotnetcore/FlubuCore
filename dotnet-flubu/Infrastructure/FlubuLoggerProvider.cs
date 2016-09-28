using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace DotNet.Cli.Flubu.Infrastructure
{
    public class FlubuLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;

        private readonly ConcurrentDictionary<string, FlubuConsoleLogger> _loggers =
            new ConcurrentDictionary<string, FlubuConsoleLogger>();

        public FlubuLoggerProvider()
        {
        }

        public FlubuLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _filter = filter;
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, CreateLoggerImplementation);
        }

        public void Dispose()
        {
        }

        private FlubuConsoleLogger CreateLoggerImplementation(string name)
        {
            return new FlubuConsoleLogger(name, GetFilter());
        }

        private Func<string, LogLevel, bool> GetFilter()
        {
            if (_filter != null)
            {
                return _filter;
            }

            return (n, l) => true;
        }
    }
}