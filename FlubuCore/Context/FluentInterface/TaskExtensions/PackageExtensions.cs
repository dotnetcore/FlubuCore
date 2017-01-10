using System.IO;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public PackageTask CreateZipPackageFromProjects(string zipPrefix, string targetFramework, params string[] folders)
        {
            var task = Context.Tasks().PackageTask(string.Empty); // must be string.Empty because of a constuctor

            task.ZipPackage(zipPrefix);

            foreach (var folder in folders)
            {
                var fullFolder = Path.Combine(folder, $"bin/Release/{targetFramework}/publish");

                task.AddDirectoryToPackage(
                    fullFolder,
                    Path.GetFileName(folder),
                    true);
            }

            Target.Target.AddTask(task);

            return task;
        }

        public PackageTask CreateZipPackage(string zipPrefix)
        {
            PackageTask task = Context.Tasks().PackageTask(string.Empty); // must be string.Empty because of a constuctor

            task
                .ZipPackage(zipPrefix);

            Target.Target.AddTask(task);

            return task;
        }
    }
}
