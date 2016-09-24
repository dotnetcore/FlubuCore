using System.Text.RegularExpressions;

namespace Flubu.Packaging
{
    public class RegexFileFilter : IFileFilter
    {
        private readonly Regex _filterRegex;

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