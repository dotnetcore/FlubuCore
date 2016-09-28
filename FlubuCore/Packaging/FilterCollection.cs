using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Flubu.Packaging
{
    public class FilterCollection : IFileFilter
    {
        private List<IFileFilter> _filters = new List<IFileFilter>();

        public FilterCollection Add(IFileFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public bool IsPassedThrough(string fileName)
        {
            foreach (IFileFilter filter in _filters)
            {
                if (!filter.IsPassedThrough(fileName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}