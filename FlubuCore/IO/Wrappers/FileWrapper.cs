using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlubuCore.IO.Wrappers
{
    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public List<string> ReadAllLines(string fileName)
        {
            return File.ReadAllLines(fileName).ToList();
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
