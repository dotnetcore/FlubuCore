using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Packaging
{
    public static class FilterExtensions
    {
        public static IFileFilter NegateFilter(this IFileFilter fileFilter)
        {
            return new NegativeFilter(fileFilter);
        }
    }
}
