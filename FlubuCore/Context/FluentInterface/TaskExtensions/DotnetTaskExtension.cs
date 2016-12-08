using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface DotnetRestore(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Restore(project));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface DotnetPublish(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Publish(project));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface DotnetBuild(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Build(project));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface DotnetBuild(string workingFolder, params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Build(project, workingFolder));
            }

            return this;
        }
    }
}
