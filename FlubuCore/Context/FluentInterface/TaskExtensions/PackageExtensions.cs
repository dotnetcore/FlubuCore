using System.IO;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface CreateSimplePackage(string zipPrefix, params string[] folders)
        {
            CreatePackage(zipPrefix, folders);
            return this;
        }

        public PackageTask CreatePackage(string zipPrefix, params string[] folders)
        {
            var task = Context.Tasks().PackageTask(string.Empty); // must be string.Empty because of a constuctor

            task.ZipPrefix(zipPrefix);

            foreach (var folder in folders)
            {
                var fullFolder = Path.Combine(folder, "bin/Release/netcoreapp1.1/publish");

                task.AddDirectoryToPackage(
                    fullFolder,
                    Path.GetFileName(folder),
                    true);
            }

            Target.Target.AddTask(task);

            return task;
        }
    }
}
