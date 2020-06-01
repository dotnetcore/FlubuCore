using System;
using FlubuCore.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting
{
    public interface IBuildScript
    {
        int Run(IFlubuSession flubuSession);

        void ConfigureServices(IServiceCollection services);

        void Configure(IFlubuConfigurationBuilder configurationBuilder, ILoggerFactory loggerFactory);
    }
}