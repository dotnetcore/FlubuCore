using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITaskExtensionsFluentInterface
    {
        /// <summary>
        /// Generate's common assembly info file. Information is taken from build properties cotnext.
        /// </summary>
        /// <returns></returns>
        ITaskExtensionsFluentInterface GenerateCommonAssemblyInfo();

        /// <summary>
        /// Run's multriple programs
        /// </summary>
        /// <param name="programs"></param>
        /// <returns></returns>
        ITaskExtensionsFluentInterface RunMultiProgram(params string[] programs);

        /// <summary>
        /// Run specified program.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="workingFolder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder, params string[] args);

        /// <summary>
        ///  Moves back to target fluent interface.
        /// </summary>
        /// <returns></returns>
        ITargetFluentInterface BackToTarget();
    }
}
