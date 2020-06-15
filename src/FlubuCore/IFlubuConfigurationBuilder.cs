using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;

namespace FlubuCore
{
    public interface IFlubuConfigurationBuilder
    {
        FlubuConfigurationBuilder ConfigureTravis(Action<TravisOptions> configuration);

        FlubuConfigurationBuilder ConfigureAzurePipelines(Action<AzurePipelineOptions> configuration);

        FlubuConfiguration Build();
    }
}
