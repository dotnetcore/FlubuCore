namespace FlubuCore.Packaging.Filters
{
    public static class FilterExtensions
    {
        public static IFileFilter NegateFilter(this IFileFilter fileFilter)
        {
            return new NegativeFilter(fileFilter);
        }
    }
}
