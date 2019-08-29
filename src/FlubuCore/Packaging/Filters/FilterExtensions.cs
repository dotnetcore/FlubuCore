namespace FlubuCore.Packaging.Filters
{
    public static class FilterExtensions
    {
        public static IFilter NegateFilter(this IFilter fileFilter)
        {
            return new NegativeFilter(fileFilter);
        }
    }
}
