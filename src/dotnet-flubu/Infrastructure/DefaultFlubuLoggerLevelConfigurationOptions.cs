using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNet.Cli.Flubu.Infrastructure
{
    public class DefaultFlubuLoggerLevelConfigurationOptions : ConfigureOptions<LoggerFilterOptions>
    {
        public DefaultFlubuLoggerLevelConfigurationOptions(LogLevel level)
            : base(options => options.MinLevel = level)
        {
        }
    }
}
