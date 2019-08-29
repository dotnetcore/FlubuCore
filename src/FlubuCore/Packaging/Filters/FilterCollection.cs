using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NuGet.Packaging;

namespace FlubuCore.Packaging
{
    public class FilterCollection : IFilter
    {
        private ICollection<IFilter> _filters = new List<IFilter>();

        public FilterCollection Add(IFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public FilterCollection AddRange(IEnumerable<IFilter> filter)
        {
            _filters.AddRange(filter);
            return this;
        }

        public bool IsPassedThrough(string path)
        {
            foreach (IFilter filter in _filters)
            {
                if (!filter.IsPassedThrough(path))
                {
                    return false;
                }
            }

            return true;
        }
    }
}