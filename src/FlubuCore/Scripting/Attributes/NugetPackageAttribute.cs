using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    /// <summary>
    /// Adds nuget package reference to script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NugetPackageAttribute : Attribute
    {
        /// <summary>
        /// Adds nuget package reference to script.
        /// </summary>
        /// <param name="packageId">Id of the nuget package. For Example "FlubuCore".</param>
        /// <param name="packageVersion">Version of the nuget package. For Example "3.0.0".</param>
        public NugetPackageAttribute(string packageId, string packageVersion)
        {
        }
    }
}
