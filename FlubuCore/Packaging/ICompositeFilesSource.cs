using System.Collections.Generic;

namespace Flubu.Packaging
{
    public interface ICompositeFilesSource : IFilesSource
    {
        ICollection<IFilesSource> ListChildSources();
    }
}