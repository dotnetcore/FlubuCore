using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace flubu.Scripting
{
    public interface IFileLoader
    {
        string LoadFile(string fileName);
    }

    public class FileLoader : IFileLoader
    {
        public string LoadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}
