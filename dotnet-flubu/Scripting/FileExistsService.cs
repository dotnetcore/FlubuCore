using System.IO;

namespace Flubu.Scripting
{
    public class FileExistsService : IFileExistsService
    {
        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }
    }
}