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

        public FlubuConfiguration(TravisOptions travisConfiguration)
        {
            TravisConfiguration = travisConfiguration;
        }

        public TravisOptions TravisConfiguration { get; private set; }

        public void CopyFrom(FlubuConfigurationBuilder builder)
        {
            TravisConfiguration = builder.TravisConfiguration;
        }
    }
}
