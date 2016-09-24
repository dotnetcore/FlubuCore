using System.Collections.Generic;

namespace Flubu.Packaging
{
    public interface IFilesSource : IFilterable
    {
        string Id { get; }

        ICollection<PackagedFileInfo> ListFiles();
    }
}