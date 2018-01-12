using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlubuCore.IO.Wrappers
{
    public class DirectoryWrapper : IDirectoryWrapper
    {
        public string[] GetFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public string[] GetFiles(string directoryPath, string searchPattern)
        {
            return Directory.GetFiles(directoryPath, searchPattern);
        }

        public string[] GetFiles(string directoryPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(directoryPath, searchPattern, searchOption);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
