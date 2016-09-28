using System.IO;

namespace DotNet.Cli.Flubu.Scripting
{
    public class FileLoader : IFileLoader
    {
        public string LoadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}