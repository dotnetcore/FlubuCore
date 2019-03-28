using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    /// <summary>
    /// Include all other c# source code file(.cs) to script from specified directory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IncludeFromDirectroyAttribute : Attribute
    {
        /// <summary>
        ///  Include all other c# source code file(.cs) to script from specified directory.
        /// </summary>
        /// <param name="directory">Directory path.</param>
        /// <param name="subDirectories">Include directory sub directories.</param>
        public IncludeFromDirectroyAttribute(string directory, bool subDirectories = false)
        {
        }
    }
}
