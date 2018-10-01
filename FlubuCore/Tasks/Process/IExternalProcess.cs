namespace FlubuCore.Tasks.Process
{
    public interface IExternalProcess<out T>
        where T : ITask
    {
        /// <summary>
        /// Set the full file path of the executable file.
        /// </summary>
        /// <param name="executableFullFilePath"></param>
        /// <returns></returns>
        T Executable(string executableFullFilePath);

        /// <summary>
        /// Add argument for executable.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="maskArg">If <c>true</c> argument is masked. Otherwise not.</param>
        /// <returns></returns>
        T WithArguments(string arg, bool maskArg);

        /// <summary>
        /// Add arguments for executable.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        T WithArguments(params string[] args);

        /// <summary>
        /// Clear all arguments for the command line.
        /// </summary>
        /// <returns></returns>
        T ClearArguments();

        /// <summary>
        /// Set the working folder for the executable.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        T WorkingFolder(string folder);

        /// <summary>
        /// Do not log output to the console.
        /// </summary>
        /// <returns></returns>
        T DoNotLogOutput();
    }
}
