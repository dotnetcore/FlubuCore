namespace Flubu.Packaging
{
    public class NegativeFilter : IFileFilter
    {
        private readonly IFileFilter _filter;

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