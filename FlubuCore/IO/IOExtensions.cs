using System.IO;

namespace FlubuCore.IO
{
    public static class IOExtensions
    {
        public static string GetFullPath(string path)
        {
            FileInfo info = new FileInfo(path);

            return info.FullName;
        }
    }
}
