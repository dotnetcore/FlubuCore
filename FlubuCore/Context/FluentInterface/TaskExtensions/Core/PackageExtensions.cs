using System;
using System.IO;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.TaskExtensions.Core
{
    public partial class CoreTaskExtensionsFluentInterface
    {
        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework, Action<PackageTask> action, params string[] projects)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, null, action, projects);
            return this;
        }


        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework, string runtime, Action<PackageTask> action, params string [] projects)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, projects);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework, string project, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project, project2);
            return this; ;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project, project2, project3);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project, project2, project3, project4);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project, project2, project3, project4, project5);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, string project6, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project, project2, project3, project4, project5, project6);
            return this;
        }

        public ICoreTaskExtensionsFluentInterface CreateZipPackageFromProjects(string zipPrefix, string targetFramework,
            string project, string project2, string project3, string project4, string project5, string project6, string project7, Action<PackageTask> action = null, string runtime = null)
        {
            CreateZipPackageFromProjectsImplementation(zipPrefix, targetFramework, runtime, action, project, project2, project3, project4, project5, project6, project7);
            return this;
        }
        
        private void CreateZipPackageFromProjectsImplementation(string zipPrefix, string targetFramework, string runtime, Action<PackageTask> action, params string[] projects)
        {
            var task = Context.Tasks().PackageTask(string.Empty); // must be string.Empty because of a constuctor

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

            Target.Target.AddTask(task);
        }
    }
}
