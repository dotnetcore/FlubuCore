using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    /// <summary>
    /// Adds assembly references to script from specified directory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AssemblyFromDirectoryAttribute : Attribute
    {
        /// <summary>
        /// Adds assembly references to script from specified directory.
        /// </summary>
        /// <param name="directory">Directory path.</param>
        /// <param name="subDirectories">Include directory sub directories.</param>
        public AssemblyFromDirectoryAttribute(string directory, bool subDirectories = false)
        {
        }
    }
}
