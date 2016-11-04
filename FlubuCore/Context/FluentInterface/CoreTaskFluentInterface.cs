using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface
{
    public class CoreTaskFluentInterface : ICoreTaskFluentInterface
    {
        public TaskContext Context { get; set; }

        public ExecuteDotnetTask ExecuteDotnetTask(string command)
        {
            return Context.CreateTask<ExecuteDotnetTask>(command);
        }

        public ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command)
        {
            return Context.CreateTask<ExecuteDotnetTask>(command);
        }

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(string filePath)
        {
            return Context.CreateTask<UpdateNetCoreVersionTask>(filePath);
        }

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files)
        {
            return Context.CreateTask<UpdateNetCoreVersionTask>(files);
        }
    }
}
