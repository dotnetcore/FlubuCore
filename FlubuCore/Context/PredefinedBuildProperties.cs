using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    /// <summary>
    /// All build properties that are predefined by flubu.
    /// </summary>
    public enum PredefinedBuildProperties
    {
        /// <summary>
        /// Current OS platform.
        /// </summary>
        OsPlatform,

        /// <summary>
        /// Path to dotnet executable cli tool
        /// </summary>
        PathToDotnetExecutable,

        /// <summary>
        /// Path to user profile folder.
        /// </summary>
        UserProfileFolder,

        /// <summary>
        /// Folder where flubu stores outputs by default. 
        /// </summary>
        OutputDir, 

        /// <summary>
        /// Root folder of the product / solution.
        /// </summary>
        ProductRootDir
    }
}
