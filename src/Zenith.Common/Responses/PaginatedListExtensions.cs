namespace Zenith.Common.Responses
{
    public static class PaginatedListExtensions
    {
        public static PaginatedList<T> ToPagedList<T>(this IEnumerable<T> source, int totalCount, int pageNumber, int pageSize)
        {
            return new PaginatedList<T>(source.ToList(), totalCount, pageNumber, pageSize);
        }
    }
}
