using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Infrastructure.ColorfulConsole;
using Microsoft.Extensions.Logging;
using Pastel;

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
        private static Color _consoleColor;

        [ThreadStatic]
        private static int _depth = 0;

        private readonly Stopwatch _stopwatch = new Stopwatch();
        //// ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? _defaultConsoleColor = null;

        private TimeSpan _lastTimeMark;

        private IColorfulConsole _console;

        public FlubuConsoleLogger(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BuildServer buildServer = new BuildServer();
            ColorfulConsole = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && buildServer.IsLocalBuild
                ? (IColorfulConsole)new WindowsConsole() : new AnsiConsole();

            _stopwatch.Start();
        }

        public static bool DisableColloredLogging { get; set; }

        public static int Depth
        {
            get { return _depth; }

            set { _depth = value; }
        }

        public static Color Color
        {
            private get
            {
                return _consoleColor;
            }

            set
            {
                _consoleColor = value;
                _useColor = true;
            }
        }

        public IColorfulConsole ColorfulConsole
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

            string timeMark = GetTimeMark();
            string indentation = Depth > 0 ? new string(' ', Depth * 3) : string.Empty;

            if (!string.IsNullOrEmpty(timeMark))
            {
                if (Depth == 0)
                {
                    timeMark = string.Empty;
                }
                else
                {
                    indentation = indentation.Length > timeMark.Length
                        ? indentation.Substring(timeMark.Length)
                        : string.Empty;
                }
            }

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
                    if (!DisableColloredLogging && !string.IsNullOrEmpty(timeMark))
                    {
                        timeMark = timeMark.Pastel(Color.Magenta);
                    }

                    if (_useColor && !DisableColloredLogging)
                    {
                        ColorfulConsole.Write($"{timeMark}{indentation}{logMessage.Pastel(Color)}", _defaultConsoleColor, _defaultConsoleColor);
                    }
                    else
                    {
                        ColorfulConsole.Write($"{timeMark}{indentation}{logMessage}", _defaultConsoleColor, _defaultConsoleColor);
                    }

                    // In case of AnsiLogConsole, the messages are not yet written to the console,
                    // this would flush them instead.
                    ColorfulConsole.Flush();

                    _useColor = false; // Disable color after each output.
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

        private string GetTimeMark()
        {
            TimeSpan timeMark = _stopwatch.Elapsed;
            TimeSpan diff = timeMark - _lastTimeMark;
            string result = string.Empty;
            if (diff.TotalSeconds >= 2)
            {
                result = string.Format("{0,1}: ", (int)diff.TotalSeconds);
            }

            _lastTimeMark = timeMark;
            return result;
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
    }
}