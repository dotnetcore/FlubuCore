using System.Collections.Generic;
using System.IO;

namespace FlubuCore.Packaging
{
    public class DirectoryFilesLister : IDirectoryFilesLister
    {
        public IEnumerable<string> ListFiles(string directoryName, bool recursive, IFilter filter)
        {
            List<string> files = new List<string>();
            ListFilesPrivate(directoryName, files, recursive, filter);
            return files;
        }

        private static void ListFilesPrivate(
            string directoryName,
            List<string> files,
            bool recursive,
            IFilter filter)
        {
            if (filter == null || filter.IsPassedThrough(directoryName))
            {
                foreach (string file in Directory.GetFiles(directoryName))
                {
                    files.Add(file);
                }
            }

            if (recursive)
            {
                foreach (string directory in Directory.GetDirectories(directoryName))
                {
                    ListFilesPrivate(directory, files, recursive, filter);
                }
            }
        }
    }
}