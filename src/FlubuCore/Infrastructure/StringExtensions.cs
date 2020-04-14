using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace FlubuCore.Infrastructure
{
    internal static class StringExtensions
    {
        public static string Fmt(this string format, params object[] args)
        {
            Contract.Requires(format != null);
            Contract.Requires(args != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string Concat<TItem>(this IEnumerable<TItem> items, Func<TItem, string> formatterFunc,
            string itemDelimiter)
        {
            Contract.Requires(items != null);
            Contract.Requires(formatterFunc != null);
            Contract.Ensures(Contract.Result<string>() != null);

            StringBuilder s = new StringBuilder();
            string actualDelimiter = null;

            foreach (TItem item in items)
            {
                s.Append(actualDelimiter);
                s.Append(formatterFunc(item));
                actualDelimiter = itemDelimiter;
            }

            return s.ToString();
        }

        public static string Capitalize(this string source)
        {
            return source.Substring(0, 1).ToUpper() + source.Substring(1);
        }
    }
}
