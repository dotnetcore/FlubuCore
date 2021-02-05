using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlubuCore.Infrastructure
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T obj in items)
                collection.Add(obj);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }

            return source.Any() == false;
        }
    }
}
