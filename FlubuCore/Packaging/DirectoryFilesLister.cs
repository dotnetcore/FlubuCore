using System.Collections.Generic;
using System.IO;

namespace FlubuCore.Packaging
{
    public class DirectoryFilesLister : IDirectoryFilesLister
    {
        public IEnumerable<string> ListFiles(string directoryName, bool recursive)
        {
            List<string> files = new List<string>();
            ListFilesPrivate(directoryName, files, recursive);
            return files;
        }

        private static void ListFilesPrivate(
            string directoryName,
            List<string> files,
            bool recursive)
        {
            foreach (string file in Directory.GetFiles(directoryName))
            {
                files.Add(file);
            }

            if (recursive)
            {
                foreach (string directory in Directory.GetDirectories(directoryName))
                {
                    ListFilesPrivate(directory, files, recursive);
                }
            }
        }
    }
}