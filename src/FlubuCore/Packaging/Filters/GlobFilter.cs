using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GlobExpressions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace FlubuCore.Packaging.Filters
{
    public class GlobFilter : IFilter
    {
        private string _pattern;

        private GlobOptions _globOptions;

        public GlobFilter(string pattern, GlobOptions globOptions = GlobOptions.None)
        {
           _pattern = pattern;
           _globOptions = globOptions;
        }

        public bool IsPassedThrough(string path)
        {
            return !Glob.IsMatch(path, _pattern, _globOptions);
        }
    }
}
