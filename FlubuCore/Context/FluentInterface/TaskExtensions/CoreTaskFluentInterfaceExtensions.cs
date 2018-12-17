using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Testing;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public static class CoreTaskFluentInterfaceExtensions
    {
        public static ExecuteDotnetTask DotnetAddEfMigration(this ICoreTaskFluentInterface coreTask, string workingFolder, string migrationName = "default", Action<ExecuteDotnetTask> taskOptions = null)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            var task = AddEfMigration(core.Context, workingFolder, migrationName, taskOptions);
            return task;
        }

        public static ExecuteDotnetTask DotnetRemoveEfMigration(this ICoreTaskFluentInterface coreTask, string workingFolder, bool forceRemove = true, Action<ExecuteDotnetTask> taskOptions = null)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            var task = RemoveEfMigration(core.Context, workingFolder, forceRemove);
            taskOptions?.Invoke(task);
            return task;
        }

        public static ExecuteDotnetTask DotnetEfUpdateDatabase(this ICoreTaskFluentInterface coreTask, string workingFolder, Action<ExecuteDotnetTask> taskOptions = null)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            var task = EfUpdateDatabase(core.Context, workingFolder);
            taskOptions?.Invoke(task);
            return task;
        }

        public static ExecuteDotnetTask DotnetEfDropDatabase(this ICoreTaskFluentInterface coreTask, string workingFolder, Action<ExecuteDotnetTask> taskOptions = null)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            var task = EfDropDatabase(core.Context, workingFolder);
            taskOptions?.Invoke(task);
            return task;
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework, Action<PackageTask> action, params string[] projects)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, null, string.Empty,
                action, projects);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework, string runtime, Action<PackageTask> packageTaskOptions, params string[] projects)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, string.Empty, packageTaskOptions, projects);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework, string runtime, string destinationRootDir, Action<PackageTask> packageTaskOptions, params string[] projects)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir, packageTaskOptions, projects);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework, string project, Action<PackageTask> packageTaskOptions = null, string runtime = null, string destinationRootDir = "")
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir, packageTaskOptions, project);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework,
            string project, string project2, Action<PackageTask> packageTaskOptions = null, string runtime = null, string destinationRootDir = "")
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir,  packageTaskOptions, project, project2);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework,
            string project, string project2, string project3, Action<PackageTask> packageTaskOptions = null, string runtime = null, string destinationRootDir = "")
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir, packageTaskOptions, project, project2, project3);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, Action<PackageTask> packageTaskOptions = null, string runtime = null, string destinationRootDir = "")
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir, packageTaskOptions, project, project2, project3, project4);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, Action<PackageTask> packageTaskOptions = null, string runtime = null, string destinationRootDir = "")
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir, packageTaskOptions, project, project2, project3, project4, project5);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(this ICoreTaskFluentInterface coreTask, string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, string project6, Action<PackageTask> action = null, string runtime = null, string destinationRootDir = "")
        {
           var core = (CoreTaskFluentInterface)coreTask;
           return CreateZipPackageFromProjectsImplementation(core.Context, zipPrefix, targetFramework, runtime, destinationRootDir, action, project, project2, project3, project4, project5, project6);
        }

        /// <summary>
        /// Create ZIP file with specified folders. Returns PackageTask to add additional properties.
        /// </summary>
        /// <param name="zipPrefix">Zip file prefix. Version will be added automatically</param>
        /// <param name="targetFramework">Framework to use for package folder. Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="projects">Name of the project to add to add</param>
        /// <param name="runtime">Runtime to use for package folder.  Folder is combined as follows {project}/bin/Release/{targetFramework}/{runtime}/publish</param>
        /// <param name="destinationRootDir">Name of the directory that the source directory will be copied to.</param>
        /// <returns><see cref="PackageTask"/> instance.</returns>
        public static PackageTask CreateZipPackageFromProjects(
            this ICoreTaskFluentInterface coreTask,
            string zipPrefix,
            string targetFramework,
            string project,
            string project2,
            string project3,
            string project4,
            string project5,
            string project6,
            string project7,
            Action<PackageTask> packageTaskOptions = null,
            string runtime = null,
            string destinationRootDir = "")
        {
            var core = (CoreTaskFluentInterface)coreTask;
           return CreateZipPackageFromProjectsImplementation(
                core.Context,
                zipPrefix,
                targetFramework,
                runtime,
                destinationRootDir,
                packageTaskOptions,
                project,
                project2,
                project3,
                project4,
                project5,
                project6,
                project7);
        }

        public static List<OpenCoverTask> DotnetCoverage(this ICoreTaskFluentInterface coreTask, params string[] projects)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            var tasks = new List<OpenCoverTask>();
            foreach (string project in projects)
            {
                tasks.Add(DotnetCoverage(core.Context, project, null, null, new string[1]));
            }

            return tasks;
        }

        public static OpenCoverTask DotnetCoverage(this ICoreTaskFluentInterface coreTask, string projectPath, string output, params string[] excludeList)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return DotnetCoverage(core.Context, projectPath, output, null, excludeList);
        }

        public static OpenCoverTask DotnetCoverage(this ICoreTaskFluentInterface coreTask, string projectPath, string[] includeList, string[] excludeList)
        {
            var core = (CoreTaskFluentInterface)coreTask;
            return DotnetCoverage(core.Context, projectPath, null, includeList, excludeList);
        }

        private static OpenCoverTask DotnetCoverage(TaskContext context, string projectPath, string output, string[] includeList, string[] excludeList)
        {
            if (string.IsNullOrEmpty(output))
                output = $"{Path.GetFileNameWithoutExtension(projectPath)}cover.xml";

            OpenCoverTask task = context.Tasks().OpenCoverTask()
                .TestExecutableArgs($"test {projectPath}")
                .Output(output)
                .UseDotNet()
                .AddExclude(excludeList);

            if (includeList == null || includeList.Length <= 0)
                task.IncludeAll();
            else
                task.AddInclude(includeList);

            return task;
        }

        private static PackageTask CreateZipPackageFromProjectsImplementation(TaskContext context, string zipPrefix, string targetFramework, string runtime, string destinationRootDir, Action<PackageTask> action, params string[] projects)
        {
            var task = context.Tasks().PackageTask(string.Empty); // must be string.Empty because of a constuctor

            task.ZipPackage(zipPrefix);

            foreach (var project in projects)
            {
                var fullproject = string.IsNullOrEmpty(runtime)
                    ? Path.Combine(project, $"bin/Release/{targetFramework}/publish")
                    : Path.Combine(project, $"bin/Release/{targetFramework}/{runtime}/publish");

                task.AddDirectoryToPackage(
                    fullproject,
                    Path.GetFileName(project),
                    true);
            }

            action?.Invoke(task);

            return task;
        }

        private static ExecuteDotnetTask AddEfMigration(TaskContext context, string workingFolder, string migrationName = "default", Action<ExecuteDotnetTask> action = null)
        {
            var task = context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "add", migrationName);

            action?.Invoke(task);

            return task;
        }

        private static ExecuteDotnetTask RemoveEfMigration(TaskContext context, string workingFolder,
            bool forceRemove = true)
        {
            var task = context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "remove");

            if (forceRemove)
                task.WithArguments("--force");

            return task;
        }

        private static ExecuteDotnetTask EfUpdateDatabase(TaskContext context, string workingFolder)
        {
            return context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "update");
        }

        private static ExecuteDotnetTask EfDropDatabase(TaskContext context, string workingFolder)
        {
            return context.CoreTasks()
                .ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "drop", "--force");
        }
    }
}
