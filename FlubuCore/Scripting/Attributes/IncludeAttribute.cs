using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    /// <summary>
    /// Include other c# source code file(.cs) to script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IncludeAttribute : Attribute
    {
        /// <summary>
        /// Include other c# source code file(.cs) to script.
        /// </summary>
        /// <param name="pathToCsFile">Path to c# source code file. </param>
        public IncludeAttribute(string pathToCsFile)
        {
        }
    }
}
