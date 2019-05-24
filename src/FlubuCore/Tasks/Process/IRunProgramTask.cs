namespace FlubuCore.Tasks.Process
{
    /// <inheritdoc cref="ITaskOfT{T, TTask}" />
    public interface IRunProgramTask : ITaskOfT<int, IRunProgramTask>, IExternalProcess<IRunProgramTask>
    {
        /// <summary>
        ///     Capture output of the running program.
        /// </summary>
        /// <returns></returns>
        IRunProgramTask CaptureOutput();

        /// <summary>
        ///     Capture error output of the running program.
        /// </summary>
        /// <returns></returns>
        IRunProgramTask CaptureErrorOutput();

        /// <summary>
        ///     Gets the whole output of the executed command.
        /// </summary>
        /// <returns></returns>
        string GetOutput();

        /// <summary>
        ///     Gets the whole error output of the executed command.
        /// </summary>
        /// <returns></returns>
        string GetErrorOutput();
    }
}