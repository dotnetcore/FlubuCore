namespace FlubuCore.Tasks.Process
{
    public interface IExternalProcess<out T> where T : ITask
    {

        /// <summary>
        /// Add argument for executable.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        T WithArguments(string arg);

        /// <summary>
        /// Add more arguments for executable.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        T WithArguments(params string[] args);

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
