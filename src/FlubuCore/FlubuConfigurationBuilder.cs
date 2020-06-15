using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;

namespace FlubuCore
{
    public class FlubuConfigurationBuilder : IFlubuConfigurationBuilder
    {
        public TravisOptions TravisConfiguration { get; set; }

        public AzurePipelineOptions AzurePipelineOptions { get; set; }

        public FlubuConfigurationBuilder ConfigureTravis(Action<TravisOptions> configuration)
        {
            if (TravisConfiguration == null)
            {
                TravisConfiguration = new TravisOptions();
            }

            configuration.Invoke(TravisConfiguration);
            return this;
        }

        public FlubuConfigurationBuilder ConfigureAzurePipelines(Action<AzurePipelineOptions> configuration)
        {
            if (AzurePipelineOptions == null)
            {
                AzurePipelineOptions = new AzurePipelineOptions();
            }

            configuration.Invoke(AzurePipelineOptions);

            return this;
        }

        public FlubuConfiguration Build()
        {
            return new FlubuConfiguration(TravisConfiguration, AzurePipelineOptions);
        }
    }
}
