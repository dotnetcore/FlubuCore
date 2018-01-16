using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Context.FluentInterface.TaskExtensions.Core
{
    public partial class CoreTaskExtensionsFluentInterface
    {
        public ICoreTaskExtensionsFluentInterface DotnetRestore(params string[] projects)
        {
           return DotnetRestore(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> taskAction, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Restore(project);
                taskAction?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> taskAction = null)
        {
            return DotnetRestore(null, null, taskAction);
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
            return DotnetPublish(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> taskAction, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Publish(project);
                taskAction?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> taskAction = null)
        {
          return DotnetPublish(null, taskAction);
        }

        public ICoreTaskExtensionsFluentInterface DotnetPublish(string project, Action<DotnetPublishTask> taskAction = null)
        {
            var task = Context.CoreTasks().Publish(project);
            taskAction?.Invoke(task);
            Target.AddTask(task);

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects)
        {
            return DotnetBuild(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> taskAction = null, params string[] projects)
        {
            return DotnetBuild(null, taskAction, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder = null, Action<DotnetBuildTask> taskAction = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Build(project, workingFolder);
                taskAction?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> taskAction = null)
        {
           return DotnetBuild(null, null, taskAction);
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(string project = null, string workingFolder = null, Action<DotnetBuildTask> taskAction = null)
        {
            var task = Context.CoreTasks().Build(project, workingFolder);
            taskAction?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(params string[] projects)
        {
            return DotnetUnitTest(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> taskAction = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Test().Project(project);
                taskAction?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> taskAction = null)
        {
            return DotnetUnitTest(null, taskAction);
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(string project, Action<DotnetTestTask> taskAction = null)
        {
            var task = Context.CoreTasks().Test().Project(project);
            taskAction?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack(params string[] projects)
        {
            return DotnetPack(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> taskAction = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Pack().Project(project);
                taskAction?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> taskAction = null)
        {
            return DotnetPack(null, taskAction);
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack(string project, Action<DotnetPackTask> action = null)
        {
            var task = Context.CoreTasks().Pack().Project(project);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetClean(params string[] projects)
        {
            return DotnetClean(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> taskOptions, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Clean().Project(project);
                taskOptions?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetClean(string project, Action<DotnetCleanTask> taskOptions = null)
        {
            var task = Context.CoreTasks().Clean().Project(project);
            taskOptions?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> taskOptions = null)
        {
           return DotnetClean(null, taskOptions);
        }

        public ICoreTaskExtensionsFluentInterface DotnetNugetPush(string nugetPackagePath, Action<DotnetNugetPushTask> taskOptions = null)
        {
            var task = Context.CoreTasks().NugetPush(nugetPackagePath);
            taskOptions?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps)
        {
            Target.AddCoreTask(x => x.UpdateNetCoreVersionTask(projectFiles)
                .AdditionalProp(additionalProps));

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetAddEfMigration(string workingFolder, string migrationName = "default", Action<ExecuteDotnetTask> taskOptions = null)
        {
            var task = AddEfMigration(workingFolder, migrationName, taskOptions);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetRemoveEfMigration(string workingFolder, bool forceRemove = true, Action<ExecuteDotnetTask> taskOptions = null)
        {
            var task = RemoveEfMigration(workingFolder, forceRemove);

            taskOptions?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetEfUpdateDatabase(string workingFolder, Action<ExecuteDotnetTask> taskOptions = null)
        {
            var task = EfUpdateDatabase(workingFolder);
            taskOptions?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetEfDropDatabase(string workingFolder, Action<ExecuteDotnetTask> taskOptions = null)
        {
            var task = EfDropDatabase(workingFolder);
            Target.AddTask(task);
            return this;
        }

        private ExecuteDotnetTask AddEfMigration(string workingFolder, string migrationName = "default", Action<ExecuteDotnetTask> action = null)
        {
            var task = Context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "add", migrationName);

            action?.Invoke(task);

            return task;
        }

        private ExecuteDotnetTask RemoveEfMigration(string workingFolder, bool forceRemove = true)
        {
            var task = Context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "remove");

            if (forceRemove)
                task.WithArguments("--force");

            return task;
        }

        private ExecuteDotnetTask EfUpdateDatabase(string workingFolder)
        {
            return Context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "update");
        }

        public ExecuteDotnetTask EfDropDatabase(string workingFolder)
        {
            return Context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "drop", "--force");
        }
    }
}
