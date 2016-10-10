using System;
using System.Collections.Generic;

namespace Flubu.VisualStudio.TaskRunner
{
    internal class TrimmingStringComparer : IEqualityComparer<string>
    {
        private readonly IEqualityComparer<string> _basisComparison;
        private readonly char _toTrim;

        public TrimmingStringComparer(char toTrim)
            : this(toTrim, StringComparer.Ordinal)
        {
        }

        public TrimmingStringComparer(char toTrim, IEqualityComparer<string> basisComparer)
        {
            _toTrim = toTrim;
            _basisComparison = basisComparer;
        }

        public bool Equals(string x, string y)
        {
            var realX = x?.TrimEnd(_toTrim);
            var realY = y?.TrimEnd(_toTrim);
            return _basisComparison.Equals(realX, realY);
        }

        public int GetHashCode(string obj)
        {
            var realObj = obj?.TrimEnd(_toTrim);
            return realObj != null ? _basisComparison.GetHashCode(realObj) : 0;
        }
    }
}