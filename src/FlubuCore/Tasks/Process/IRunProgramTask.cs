using System;
using System.Collections.Generic;

namespace FlubuCore.Tasks.Process
{
    /// <inheritdoc cref="ITaskOfT{T, TTask}" />
    public interface IRunProgramTask : ITaskOfT<int, IRunProgramTask>, IExternalProcess<IRunProgramTask>
    {
        /// <summary>
        /// Add argument for executable.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="maskArg">If <c>true</c> argument is masked. Otherwise not.</param>
        /// <returns></returns>
        new IRunProgramTask WithArguments(string arg, bool maskArg);

        /// <summary>
        /// Add arguments for executable.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        new IRunProgramTask WithArguments(params string[] args);

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

        /// <summary>
        /// When applied task execution info is not logged.
        /// </summary>
        IRunProgramTask DoNotLogTaskExecutionInfo();

        IRunProgramTask ChangeDefaultAdditionalOptionPrefix(string newPrefix);

        IRunProgramTask AddNewAdditionalOptionPrefix(string newPrefix);

        IRunProgramTask AddNewAdditionalOptionPrefix(List<string> newPrefixes);

        IRunProgramTask ChangeAdditionalOptionKeyValueSeperator(char newSeperator);

        IRunProgramTask AddPrefixToAdditionalOptionKey(Func<string, string> action);
    }
}