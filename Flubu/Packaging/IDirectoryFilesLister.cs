using System.Collections.Generic;

namespace Flubu.Packaging
{
    public interface IDirectoryFilesLister
    {
        IEnumerable<string> ListFiles(string directoryName, bool recursive);
    }
}