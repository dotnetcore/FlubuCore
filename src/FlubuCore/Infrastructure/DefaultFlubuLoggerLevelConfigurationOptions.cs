using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlubuCore.Infrastructure
{
    public class DefaultFlubuLoggerLevelConfigurationOptions : ConfigureOptions<LoggerFilterOptions>
    {
        public DefaultFlubuLoggerLevelConfigurationOptions(LogLevel level)
            : base(options => options.MinLevel = level)
        {
        }
    }
}
