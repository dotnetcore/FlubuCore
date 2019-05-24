using System.Collections.Generic;

namespace FlubuCore.Packaging
{
    public interface ICompositeFilesSource : IFilesSource
    {
        ICollection<IFilesSource> ListChildSources();
    }
}