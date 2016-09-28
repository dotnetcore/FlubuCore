using System.Collections.Generic;

namespace FlubuCore.Packaging
{
    public interface IFilesSource : IFilterable
    {
        string Id { get; }

        ICollection<PackagedFileInfo> ListFiles();
    }
}