using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface
{
    public interface ICoreTaskFluentInterface
    {
        TaskContext Context { get; set; }

        ExecuteDotnetTask ExecuteDotnetTask(string command);

        ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command);

        UpdateNetCoreVersionTask UpdateNetCoreVersionTask(string filePath);

        UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files);
    }
}
