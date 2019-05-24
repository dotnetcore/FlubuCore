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
    }
}
