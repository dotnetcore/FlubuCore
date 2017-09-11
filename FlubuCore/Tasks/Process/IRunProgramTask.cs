namespace FlubuCore.Tasks.Process
{
    /// <inheritdoc />
    public interface IRunProgramTask : ITaskOfT<int>
    {
        /// <summary>
        /// Add argument for executable.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        IRunProgramTask WithArguments(string arg);

        /// <summary>
        /// Add more arguments for executable.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IRunProgramTask WithArguments(params string[] args);

        /// <summary>
        /// Set the working folder for the executable.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        IRunProgramTask WorkingFolder(string folder);

        /// <summary>
        /// Capture output of the running program.
        /// </summary>
        /// <returns></returns>
        IRunProgramTask CaptureOutput();

        /// <summary>
        /// Capture error output of the running program.
        /// </summary>
        /// <returns></returns>
        IRunProgramTask CaptureErrorOutput();

        string GetOutput();
        string GetErrorOutput();
    }
}
