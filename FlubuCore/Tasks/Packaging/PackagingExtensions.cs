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

        public static ITarget Package(this ITarget target, string zipPath, params string[] folders)
        {
            PackageTask task = new PackageTask("output")
                .ZipPackage(zipPath);

            foreach (var folder in folders)
            {
                task.AddDirectoryToPackage(
                    folder.GetHashCode().ToString(),
                    folder,
                    Path.GetDirectoryName(folder),
                    true);
            }

            return target.AddTask(task);
        }

        public static UnzipTask Unzip(string zip, string destination)
        {
            return new UnzipTask(zip, destination);
        }
    }
}
