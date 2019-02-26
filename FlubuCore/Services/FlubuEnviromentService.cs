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
            SortedDictionary<Version, string> toolsVersions = new SortedDictionary<Version, string>();
            FlubuEnviroment.FillVersionsFromMsBuildToolsVersionsRegPath(toolsVersions);
            FlubuEnviroment.FillVersion15FromVisualStudio2017(toolsVersions);
            FlubuEnviroment.FillMsBuild16Path(toolsVersions);
            return toolsVersions;
        }
    }
}
