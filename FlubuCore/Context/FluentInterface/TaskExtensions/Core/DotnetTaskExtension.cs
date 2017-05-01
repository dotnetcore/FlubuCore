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

        public ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Restore(project);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetRestore(Action<DotnetRestoreTask> action = null)
        {
            return DotnetRestore(null, null, action);
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

        public ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> action, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Publish(project);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPublish(Action<DotnetPublishTask> action = null)
        {
          return DotnetPublish(null, action);
        }

        public ICoreTaskExtensionsFluentInterface DotnetPublish(string project, Action<DotnetPublishTask> action = null)
        {
            var task = Context.CoreTasks().Publish(project);
            action?.Invoke(task);
            Target.AddTask(task);

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(params string[] projects)
        {
            return DotnetBuild(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(Action<DotnetBuildTask> action = null, params string[] projects)
        {
            return DotnetBuild(null, action, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(string workingFolder = null, Action<DotnetBuildTask> action = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Build(project, workingFolder);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild( Action<DotnetBuildTask> action = null)
        {
           return DotnetBuild(null, null, action);
        }

        public ICoreTaskExtensionsFluentInterface DotnetBuild(string project = null, string workingFolder = null, Action<DotnetBuildTask> action = null)
        {
            var task = Context.CoreTasks().Build(project, workingFolder);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(params string[] projects)
        {
            return DotnetUnitTest(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> action = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Test().Project(project);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(Action<DotnetTestTask> action = null)
        {
            return DotnetUnitTest(null, action);
        }

        public ICoreTaskExtensionsFluentInterface DotnetUnitTest(string project, Action<DotnetTestTask> action = null)
        {
            var task = Context.CoreTasks().Test().Project(project);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack( params string[] projects)
        {
            return DotnetPack(null, projects);
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> action = null, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Pack().Project(project);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetPack(Action<DotnetPackTask> action = null)
        {
            return DotnetPack(null, action);
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

        public ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> action, params string[] projects)
        {
            foreach (string project in projects)
            {
                var task = Context.CoreTasks().Clean().Project(project);
                action?.Invoke(task);
                Target.AddTask(task);
            }

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetClean(string project, Action<DotnetCleanTask> action = null)
        {
            var task = Context.CoreTasks().Clean().Project(project);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetClean(Action<DotnetCleanTask> action = null)
        {
           return DotnetClean(null, action);
        }

        public ICoreTaskExtensionsFluentInterface DotnetNugetPush(string nugetPackagePath, Action<DotnetNugetPushTask> action = null)
        {
            var task = Context.CoreTasks().NugetPush(nugetPackagePath);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }


        public ICoreTaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps)
        {
            Target.AddCoreTask(x => x.UpdateNetCoreVersionTask(projectFiles)
                .AdditionalProp(additionalProps));

            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetAddEfMigration(string workingFolder, string migrationName = "default", Action<ExecuteDotnetTask> action = null)
        {
            var task = AddEfMigration(workingFolder, migrationName, action);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetRemoveEfMigration(string workingFolder, bool forceRemove = true, Action<ExecuteDotnetTask> action = null)
        {
            var task = RemoveEfMigration(workingFolder, forceRemove);

            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetEfUpdateDatabase(string workingFolder, Action<ExecuteDotnetTask> action = null)
        {
            var task = EfUpdateDatabase(workingFolder);
            action?.Invoke(task);
            Target.AddTask(task);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface DotnetEfDropDatabase(string workingFolder, Action<ExecuteDotnetTask> action = null)
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
