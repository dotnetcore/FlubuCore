using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ICoreTaskFluentInterface
    {
        ILinuxTaskFluentInterface LinuxTasks();

        ExecuteDotnetTask ExecuteDotnetTask(string command);

        ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command);

        UpdateNetCoreVersionTask UpdateNetCoreVersionTask(string filePath);

        UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files);

        ExecuteDotnetTask Restore(string projectName = null, string workingFolder = null);

        ExecuteDotnetTask Publish(
            string projectName = null,
            string workingFolder = null,
            string configuration = "Release");

        ExecuteDotnetTask Build(string projectName = null, string workingFolder = null);
    }
}
