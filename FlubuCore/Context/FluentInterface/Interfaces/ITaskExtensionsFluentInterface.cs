using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITaskExtensionsFluentInterface
    {
        ITaskExtensionsFluentInterface GenerateCommonAssemblyInfo();

        ITaskExtensionsFluentInterface RunMultiProgram(params string[] programs);

        ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder, params string[] args);

        ITargetFluentInterface BackToTarget();
    }
}
