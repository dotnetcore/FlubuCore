using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    /// <summary>
    /// Loads System assemblies to script by fully qualified assembly name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ReferenceAttribute : Attribute
    {
        /// <summary>
        /// Loads System assemblies to script by fully qualified assembly name.
        /// </summary>
        /// <param name="fullyQualifedAssemblyName">The fully qualifed assembly name.</param>
        public ReferenceAttribute(string fullyQualifedAssemblyName)
        {
        }
    }
}
