using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlubuCore.IO.Wrappers
{
    public interface IDirectoryWrapper
    {
        string[] GetFiles(string directoryPath);

        string[] GetFiles(string directoryPath, string searchPattern);

        string[] GetFiles(string directoryPath, string searchPattern, SearchOption searchOption);

        bool Exists(string path);
    }
}
