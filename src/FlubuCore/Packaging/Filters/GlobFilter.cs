using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Globbing;
using Microsoft.Extensions.FileSystemGlobbing;

namespace FlubuCore.Packaging.Filters
{
    public class GlobFilter : IFileFilter
    {
        private Glob _glob;

        public GlobFilter(string pattern)
        {
           _glob = Glob.Parse(pattern);
        }

        public bool IsPassedThrough(string fileName)
        {
           return !_glob.IsMatch(fileName);
        }
    }
}
