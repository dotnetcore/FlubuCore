namespace FlubuCore.Context
{
    /// <summary>
    ///     All build properties that are predefined by flubu.
    /// </summary>
    public enum PredefinedBuildProperties
    {
        /// <summary>
        ///     Current OS platform.
        /// </summary>
        OsPlatform,

        /// <summary>
        ///     Path to dotnet executable cli tool
        /// </summary>
        PathToDotnetExecutable,

        /// <summary>
        ///     Path to user profile folder.
        /// </summary>
        UserProfileFolder,

        /// <summary>
        ///     Folder where flubu stores outputs by default.
        /// </summary>
        OutputDir,

        /// <summary>
        ///     Root folder of the product / solution.
        /// </summary>
        ProductRootDir,

        /// <summary>
        /// If <c>true</c> script is executed through flubu we api. Otherwise not.
        /// </summary>
        IsWebApi,
    }
}