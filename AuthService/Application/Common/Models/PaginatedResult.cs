using System.Text.Json.Serialization;

namespace AuthService.Application.Common.Models
{
    public class PaginatedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedResult() { }

        [JsonConstructor]
        public PaginatedResult(IReadOnlyList<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            Items = items ?? Array.Empty<T>();
        }

        public static PaginatedResult<T> Empty(int pageIndex, int pageSize)
            => new(Array.Empty<T>(), 0, pageIndex, pageSize);
    }
}
