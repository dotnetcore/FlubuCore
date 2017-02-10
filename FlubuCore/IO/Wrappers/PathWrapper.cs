using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlubuCore.IO.Wrappers
{
    public class PathWrapper : IPathWrapper
    {
        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }
    }
}
