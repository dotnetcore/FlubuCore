using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console.Internal;

namespace FlubuCore.Infrastructure
{
    public class FlubuConsoleLogger : ILogger
    {
        private static readonly object Lock = new object();

        [ThreadStatic]
        private static StringBuilder _logBuilder;

        [ThreadStatic]
        private static bool _useColor;

        [ThreadStatic]
        private static ConsoleColors _consoleColor;

        // ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? _defaultConsoleColor = null;

        private IConsole _console;

        public FlubuConsoleLogger(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            Console = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? (IConsole)new WindowsLogConsole()
                : new AnsiLogConsole(new AnsiSystemConsole());
        }

       public static ConsoleColors Color
       {
           private get
           {
               _useColor = false;
               return _consoleColor;
           }

           set
           {
               _consoleColor = value;
               _useColor = true;
           }
       }

        public IConsole Console
        {
            get => _console;

            set => _console = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || (exception != null))
            {
                WriteMessage(logLevel, Name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(
            LogLevel logLevel,
            string logName,
            int eventId,
            string message,
            Exception exception)
        {
            var logBuilder = _logBuilder;

            _logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            var logLevelColors = default(ConsoleColors);
            var logLevelString = string.Empty;

            if (!string.IsNullOrEmpty(message))
            {
                ////logLevelColors = GetLogLevelConsoleColors(logLevel);
                logBuilder.AppendLine(message);
            }

            if (exception != null)
            {
                logBuilder.AppendLine(exception.ToString());
            }

            if (logBuilder.Length > 0)
            {
                var logMessage = logBuilder.ToString();

                lock (Lock)
                {
                    if (_useColor)
                    {
                        // use default colors from here on
                        Console.Write(logMessage, Color.Background, Color.Foreground);
                    }
                    else
                    {
                        Console.Write(logMessage, _defaultConsoleColor, _defaultConsoleColor);
                    }

                    // In case of AnsiLogConsole, the messages are not yet written to the console,
                    // this would flush them instead.
                    Console.Flush();
                }
            }

            logBuilder.Clear();

            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }

            _logBuilder = logBuilder;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);

                case LogLevel.Error:
                    return new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red);

                case LogLevel.Warning:
                    return new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black);

                case LogLevel.Information:
                    return new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black);

                case LogLevel.Debug:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);

                case LogLevel.Trace:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);

                default:
                    return new ConsoleColors(_defaultConsoleColor, _defaultConsoleColor);
            }
        }

        public struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }

        private class AnsiSystemConsole : IAnsiSystemConsole
        {
            public void Write(string message)
            {
                System.Console.Write(message);
            }

            public void WriteLine(string message)
            {
                System.Console.WriteLine(message);
            }
        }
    }
}