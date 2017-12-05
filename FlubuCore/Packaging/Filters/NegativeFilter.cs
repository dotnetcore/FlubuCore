namespace FlubuCore.Packaging
{
    public class NegativeFilter : IFileFilter
    {
        private readonly IFileFilter _filter;

        /// <summary>
        /// Neagatives given filter.
        /// </summary>
        /// <param name="filter">Filter to be negatived.</param>
        public NegativeFilter(IFileFilter filter)
        {
            _filter = filter;
        }

        public bool IsPassedThrough(string fileName)
        {
            return !_filter.IsPassedThrough(fileName);
        }
    }
}