using System.Collections.Generic;

namespace FlubuCore.Packaging
{
    public interface IDirectoryFilesLister
    {
        IEnumerable<string> ListFiles(string directoryName, bool recursive, IFilter filter);
    }
}