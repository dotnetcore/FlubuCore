using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    /// <summary>
    /// Adds assembly reference to script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AssemblyAttribute : Attribute
    {
        /// <summary>
        /// Adds assembly reference to script.
        /// </summary>
        /// <param name="pathToAssembly">Relative or absolute path to assembly.</param>
        public AssemblyAttribute(string pathToAssembly)
        {
        }
    }
}
