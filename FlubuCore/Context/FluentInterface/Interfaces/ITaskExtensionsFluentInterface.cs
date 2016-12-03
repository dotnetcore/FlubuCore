using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ITaskExtensionsFluentInterface
    {
        ITargetFluentInterface Target { set; }

        ITaskContextInternal Context { set; }

        ITaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps);

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="folders">Folders to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        PackageTask CreatePackage(string zipPrefix, params string[] folders);

        /// <summary>
        /// Create ZIP file with specified folders.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="folders">Folders to add</param>
        /// <returns>This same instance of <see cref="ITargetFluentInterface" />.</returns>
        ITaskExtensionsFluentInterface CreateSimplePackage(string zipPrefix, params string[] folders);

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
