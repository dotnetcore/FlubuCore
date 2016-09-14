using System.IO;

namespace flubu.Scripting
{
    public interface IFileExistsService
    {
        bool FileExists(string fileName);
    }

    public class FileExistsService : IFileExistsService
    {
        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }
    }
}