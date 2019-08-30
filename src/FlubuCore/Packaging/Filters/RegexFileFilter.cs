using System.Text.RegularExpressions;

namespace FlubuCore.Packaging
{
    public class RegexFileFilter : IFilter
    {
        private readonly Regex _filterRegex;

        /// <summary>
        /// Filter files by regex expression.
        /// </summary>
        /// <param name="filterRegexValue">The regex expression.</param>
        public RegexFileFilter(string filterRegexValue)
        {
            _filterRegex = new Regex(filterRegexValue, RegexOptions.IgnoreCase);
        }

        public bool IsPassedThrough(string fileName)
        {
            return !_filterRegex.IsMatch(fileName);
        }
    }
}