using System.IO;

namespace FlubuCore.IO
{
    public class FileFullPath : IPathBuilder
    {
        private readonly string _fileName;

        public FileFullPath(string fileName)
        {
           _fileName = Path.IsPathRooted(fileName) ? fileName : Path.GetFullPath(fileName);
        }

        public FullPath Directory => ToFullPath().ParentPath;

        public bool Exists => File.Exists(_fileName);

        public string FileName => Path.GetFileName(_fileName);

        public int Length => _fileName.Length;

        public static implicit operator string(FileFullPath path)
        {
            return path.ToString();
        }

        public override string ToString()
        {
            return _fileName;
        }

        public FullPath ToFullPath()
        {
            return new FullPath(_fileName);
        }
    }
}
