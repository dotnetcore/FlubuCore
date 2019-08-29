using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Globbing;
using Microsoft.Extensions.FileSystemGlobbing;

namespace FlubuCore.Packaging.Filters
{
    public class GlobFilter : IFilter
    {
        private Glob _glob;

        public GlobFilter(string pattern)
        {
           _glob = Glob.Parse(pattern);
        }

        public bool IsPassedThrough(string path)
        {
           return !_glob.IsMatch(path);
        }
    }
}
