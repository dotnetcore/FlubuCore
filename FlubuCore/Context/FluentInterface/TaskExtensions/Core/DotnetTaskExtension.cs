using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Context.FluentInterface.TaskExtensions.Core
{
    public partial class CoreTaskExtensionsFluentInterface
    {
        public ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Restore(project);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetRestore(string project = null, string workingFolder = null, Action<DotnetRestoreTask> action = null)
        {
            var task = Context.CoreTasks().Restore(project, workingFolder);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPublish(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Publish(project));
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Build(project));
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder, params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.AddTask(Context.CoreTasks().Build(project, workingFolder));
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(string projects, Action<ExecuteDotnetTask> action)
        {

            var task = Context.CoreTasks().Build(projects);
            action(task);
            Target.AddTask();


            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.Target.AddTask(Dotnet.Test(project));
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps)
        {
            Target.AddCoreTask(x => x.UpdateNetCoreVersionTask(projectFiles)
                .AdditionalProp(additionalProps));

            return this;
        }
    }
}
