using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Services
{
    /// <summary>
    /// An abstraction layer for various <see cref="FlubuEnvironment"/> utility methods.
    /// </summary>
    public interface IFlubuEnvironmentService
    {
        /// <summary>
        /// Returns a sorted dictionary of all MSBuild tools versions that are available on the system.
        /// </summary>
        /// <remarks>The method scans through the registry (<c>HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions</c> path)
        /// to find the available tools versions.</remarks>
        /// <returns>A sorted dictionary whose keys are tools versions (2.0, 3.5, 4.0, 12.0 etc.) and values are paths to the
        /// tools directories (and NOT the <c>MSBuild.exe</c> itself!). The entries are sorted ascendingly by version numbers.</returns>
        IDictionary<Version, string> ListAvailableMSBuildToolsVersions();

        T GetEnvironmentVariable<T>(string variable);

        string GetEnvironmentVariable(string variable);
    }
}
