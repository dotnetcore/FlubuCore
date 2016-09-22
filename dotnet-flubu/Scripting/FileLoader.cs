using System.IO;

namespace Flubu.Scripting
{
    public class FileLoader : IFileLoader
    {
        public string LoadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}