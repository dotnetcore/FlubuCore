using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Context
{
    /// <summary>
    /// Target to be added 
    /// </summary>
    public enum DefaultTargets
    {
        /// <summary>
        /// Noone of the tearges are added
        /// </summary>
        None,

        /// <summary>
        /// Default dotnet target are added (compile...)
        /// </summary>
        Dotnet,

        /// <summary>
        /// Default dotnet core targets are added
        /// </summary>
        Core,
    }
}
