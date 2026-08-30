using AuthService.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Common.Extensions
{
    public static class PaginationExtensions
    {
        // 1. Phân trang bất đồng bộ cho Database Query (EF Core)
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var validPageIndex = Math.Max(1, pageIndex);
            var validPageSize = Math.Max(1, pageSize);

            var count = await source.CountAsync(cancellationToken);
            if (count == 0)
            {
                return PaginatedResult<T>.Empty(validPageIndex, validPageSize);
            }

            var items = await source
                .Skip((validPageIndex - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<T>(items, count, validPageIndex, validPageSize);
        }

        // 2. Phân trang đồng bộ cho Dữ liệu trong Memory / Redis Cache List
        public static PaginatedResult<T> ToPaginatedList<T>(
            this IEnumerable<T> source,
            int pageIndex,
            int pageSize)
        {
            var validPageIndex = Math.Max(1, pageIndex);
            var validPageSize = Math.Max(1, pageSize);

            var count = source.Count();
            if (count == 0)
            {
                return PaginatedResult<T>.Empty(validPageIndex, validPageSize);
            }

            var items = source
                .Skip((validPageIndex - 1) * validPageSize)
                .Take(validPageSize)
                .ToList();

            return new PaginatedResult<T>(items, count, validPageIndex, validPageSize);
        }
    }
}
