using FlubuCore.Targeting;

namespace FlubuCore.Tasks.FileSystem
{
    public static class FileSystemExtensions
    {
        public static ITarget CopyFile(this ITarget target, string source, string destination, bool overwrite)
        {
            return target.AddTask(CopyFile(source, destination, overwrite));
        }

        public static ITarget CopyDirectory(this ITarget target, string source, string destination, bool overwrite)
        {
            return target.AddTask(CopyDirectory(source, destination, overwrite));
        }

        public static ITarget DeleteDirectory(this ITarget target, string path, bool failIfNotExist)
        {
            return target.AddTask(new DeleteDirectoryTask(path, failIfNotExist));
        }

        public static ITarget CreateDirectory(this ITarget target, string path, bool forceRecreate)
        {
            return target.AddTask(new CreateDirectoryTask(path, forceRecreate));
        }

        public static CopyFileTask CopyFile(
            string sourceFileName,
            string destinationFileName,
            bool overwrite)
        {
            return new CopyFileTask(sourceFileName, destinationFileName, overwrite);
        }

        /// <summary>
        ///     Copies a directory tree from the source to the destination.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwriteExisting">if set to <c>true</c> the task will overwrite existing destination files.</param>
        public static CopyDirectoryStructureTask CopyDirectory(
            string sourcePath,
            string destinationPath,
            bool overwriteExisting)
        {
            return new CopyDirectoryStructureTask(sourcePath, destinationPath, overwriteExisting);
        }
    }
}
