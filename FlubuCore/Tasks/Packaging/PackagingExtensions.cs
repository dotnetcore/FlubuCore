using System.IO;
using FlubuCore.Targeting;

namespace FlubuCore.Tasks.Packaging
{
    public static class PackagingExtensions
    {
        public static ITarget Unzip(this ITarget target, string zip, string destination)
        {
            return target.AddTask(Unzip(zip, destination));
        }

        public static ITarget DotnetPackage(this ITarget target, string zipPath, params string[] folders)
        {
            target.CreateDotnetPackage(zipPath, folders);
            return target;
        }

        public static PackageTask CreateDotnetPackage(this ITarget target, string zipPrefix, params string[] folders)
        {
            PackageTask task = new PackageTask()
                .ZipPrefix(zipPrefix);

            foreach (var folder in folders)
            {
                var fullFolder = Path.Combine(folder, "bin/Release/netcoreapp1.1/publish");

                task.AddDirectoryToPackage(
                    folder.GetHashCode().ToString(),
                    fullFolder,
                    Path.GetFileName(folder),
                    true);
            }

            target.AddTask(task);
            return task;
        }

        public static UnzipTask Unzip(string zip, string destination)
        {
            return new UnzipTask(zip, destination);
        }
    }
}
