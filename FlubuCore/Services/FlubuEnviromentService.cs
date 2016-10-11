using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Services
{
    public class FlubuEnviromentService : IFlubuEnviromentService
    {
        public IDictionary<Version, string> ListAvailableMSBuildToolsVersions()
        {
           return FlubuEnviroment.ListAvailableMSBuildToolsVersions();
        }
    }
}
