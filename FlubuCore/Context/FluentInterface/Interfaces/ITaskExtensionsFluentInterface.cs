using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITaskExtensionsFluentInterface
    {
        ITargetFluentInterface Target { set; }

        ITaskContextInternal Context { set; }

        ITaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        PackageTask CreateDotnetPackage(string zipPrefix, params string[] folders);

        ITaskExtensionsFluentInterface DotnetPackage(string zipPath, params string[] folders);

        ITaskExtensionsFluentInterface RunMultiProgram(params string[] programs);

        ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder, params string[] args);

        ITaskExtensionsFluentInterface DotnetUnitTest(params string[] projects);

        ITaskExtensionsFluentInterface DotnetCoverage(params string[] projects);

        ITaskExtensionsFluentInterface DotnetCoverage(string projectPath, string output, params string[] excludeList);

        ITaskExtensionsFluentInterface DotnetCoverage(string projectPath, string[] includeList, string[] excludeList);

        ITaskExtensionsFluentInterface DotnetRestore(params string[] projects);

        ITaskExtensionsFluentInterface DotnetPublish(params string[] projects);

        ITaskExtensionsFluentInterface DotnetBuild(params string[] projects);
    }
}
