namespace FlubuCore.Packaging
{
    public class NegativeFilter : IFilter
    {
        private readonly IFilter _filter;

        /// <summary>
        /// Neagatives given filter.
        /// </summary>
        /// <param name="filter">Filter to be negatived.</param>
        public NegativeFilter(IFilter filter)
        {
            _filter = filter;
        }

        public bool IsPassedThrough(string path)
        {
            return !_filter.IsPassedThrough(path);
        }
    }
}