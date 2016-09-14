using Microsoft.Extensions.Logging;

namespace flubu.core.Infrastructure
{
    public static class LoggingExtensions
    {
        public static void Log(this ILogger log, string message)
        {
            log.LogInformation(message);
        }

        public static void Log(this ILogger log, string messageFormat, params string[] parameters)
        {
            log.LogInformation(messageFormat, parameters);
        }

    }
}
