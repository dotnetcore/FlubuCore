using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Services
{
    public class FlubuEnvironmentService : IFlubuEnvironmentService
    {
        public IDictionary<Version, string> ListAvailableMSBuildToolsVersions()
        {
            SortedDictionary<Version, string> toolsVersions = new SortedDictionary<Version, string>();
            FlubuEnvironment.FillVersionsFromMsBuildToolsVersionsRegPath(toolsVersions);
            FlubuEnvironment.FillVersion15FromVisualStudio2017(toolsVersions);
            FlubuEnvironment.FillMsBuild16Path(toolsVersions);
            return toolsVersions;
        }

        public T GetEnvironmentVariable<T>(string variable)
        {
            return FlubuEnvironment.GetEnvironmentVariable<T>(variable);
        }

        public string GetEnvironmentVariable(string variable)
        {
            return FlubuEnvironment.GetEnvironmentVariable(variable);
        }
    }
}
