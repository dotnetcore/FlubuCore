using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;

namespace FlubuCore
{
    public interface IFlubuConfigurationBuilder
    {
        FlubuConfigurationBuilder ConfigureTravis(Action<TravisOptions> configuration);

        FlubuConfiguration Build();
    }
}
