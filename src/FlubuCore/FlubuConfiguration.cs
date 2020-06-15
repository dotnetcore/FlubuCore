using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;

namespace FlubuCore
{
    public class FlubuConfiguration
    {
        protected internal FlubuConfiguration()
        {
        }

        public FlubuConfiguration(TravisOptions travisOptions, AzurePipelineOptions azureOptions)
        {
            TravisOptions = travisOptions;
            AzurePipelineOptions = azureOptions;
        }

        public TravisOptions TravisOptions { get; private set; }

        public AzurePipelineOptions AzurePipelineOptions { get; private set; }

        public void CopyFrom(FlubuConfigurationBuilder builder)
        {
            TravisOptions = builder.TravisConfiguration;
            AzurePipelineOptions = builder.AzurePipelineOptions;
        }
    }
}
