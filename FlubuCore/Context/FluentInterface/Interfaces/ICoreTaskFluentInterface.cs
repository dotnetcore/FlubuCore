using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ICoreTaskFluentInterface
    {
        ILinuxTaskFluentInterface LinuxTasks();

        ExecuteDotnetTask ExecuteDotnetTask(string command);

        ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command);

        UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files);

        DotnetRestoreTask Restore(string projectName = null, string workingFolder = null);

        DotnetPublishTask Publish(
            string projectName = null,
            string workingFolder = null,
            string configuration = "Release");

        DotnetBuildTask Build(string projectName = null, string workingFolder = null);

        DotnetPackTask Pack();

        DotnetTestTask Test();

        DotnetCleanTask Clean();
    }
}
