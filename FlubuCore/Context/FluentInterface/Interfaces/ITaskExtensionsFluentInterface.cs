using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITaskExtensionsFluentInterface
    {
        ITaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        PackageTask CreateDotnetPackage(string zipPrefix, params string[] folders);

        ITaskExtensionsFluentInterface DotnetPackage(string zipPath, params string[] folders);

        ITaskExtensionsFluentInterface RunMultiProgram(params string[] programs);

        ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder, params string[] args);

        ITaskExtensionsFluentInterface DotnetUnitTest(params string[] projects);

        ITaskExtensionsFluentInterface DotnetCoverage(params string[] projects);

        ITaskExtensionsFluentInterface DotnetCoverage(string projectPath, string output, params string[] excludeList);
    }
}
