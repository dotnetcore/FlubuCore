using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlubuCore.Packaging.Filters
{
    public class FilterOptions
    {
        internal List<IFilter> FileFilters { get; } = new List<IFilter>();

        internal List<IFilter> DirectoryFilters { get; } = new List<IFilter>();

        /// <summary>
        /// If <c>true</c> subfolders in the source directory are also added. Otherwise not.
        /// </summary>
        public bool Recursive { get; set; }

        public void AddFileFilters(params IFilter[] fileFilters)
        {
            FileFilters.AddRange(fileFilters);
        }

        public void AddDirectoryFilters(params IFilter[] directoryFilter)
        {
            DirectoryFilters.AddRange(directoryFilter);
        }
    }
}
