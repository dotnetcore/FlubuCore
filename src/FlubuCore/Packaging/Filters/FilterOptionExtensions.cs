using System;
using System.Collections.Generic;
using System.Text;
using GlobExpressions;

namespace FlubuCore.Packaging.Filters
{
    public static class FilterOptionExtensions
    {
        public static void AddFileFilterRegex(this FilterOptions options, string regexPattern, bool negateFilter = false)
        {
            var filter = new RegexFilter(regexPattern);
            if (negateFilter)
            {
                filter.NegateFilter();
            }

            options.AddFileFilters(filter);
        }

        public static void AddFileFilterGlob(this FilterOptions options, string globPattern, GlobOptions globOptions = GlobOptions.None, bool negateFilter = false)
        {
            var filter = new GlobFilter(globPattern, globOptions);
            if (negateFilter)
            {
                filter.NegateFilter();
            }

            options.AddFileFilters(filter);
        }

        public static void AddDirectoryFilterRegex(this FilterOptions options, string regexPattern, bool negateFilter = false)
        {
            var filter = new RegexFilter(regexPattern);
            if (negateFilter)
            {
                filter.NegateFilter();
            }

            options.AddDirectoryFilters(filter);
        }

        public static void AddDirectoryFilterGlob(this FilterOptions options, string globPattern, GlobOptions globOptions = GlobOptions.None, bool negateFilter = false)
        {
            var filter = new GlobFilter(globPattern, globOptions);
            if (negateFilter)
            {
                filter.NegateFilter();
            }

            options.AddDirectoryFilters(filter);
        }
    }
}
