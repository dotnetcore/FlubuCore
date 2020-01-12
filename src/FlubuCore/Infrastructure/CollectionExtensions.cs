using System;
using System.Collections.Generic;
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
    }
}
