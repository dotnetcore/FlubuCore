using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.IO
{
    public class FileFullPath : IPathBuilder
    {
        private readonly string _fileName;

        public FileFullPath(string fileName)
        {
           _fileName = Path.IsPathRooted(fileName) ? fileName : Path.GetFullPath(fileName);
        }

        public FullPath Directory
        {
            get { return ToFullPath().ParentPath; }
        }

        public bool Exists
        {
            get { return File.Exists(_fileName); }
        }

        public string FileName
        {
            get { return Path.GetFileName(_fileName); }
        }

        public int Length
        {
            get { return _fileName.Length; }
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
