using System.Collections.Generic;
using System.IO;

namespace FlubuCore.IO.Wrappers
{
    /// <summary>
    /// Wrapper interface for <see cref="File"/>
    /// </summary>
    public interface IFileWrapper
    {
        bool Exists(string path);

        string ReadAllText(string path);

        List<string> ReadAllLines(string fileName);
    }
}
