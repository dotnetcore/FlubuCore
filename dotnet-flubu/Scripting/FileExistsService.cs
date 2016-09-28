using System.IO;

namespace DotNet.Cli.Flubu.Scripting
{
    public class FileExistsService : IFileExistsService
    {
        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }
    }
}