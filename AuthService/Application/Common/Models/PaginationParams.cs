namespace AuthService.Application.Common.Models
{
    public class PaginationParams
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;
        private int _pageIndex = 1;

        public int PageIndex
        {
            get => _pageIndex;
            init => _pageIndex = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
        }
    }
}
