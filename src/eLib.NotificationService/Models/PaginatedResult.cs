namespace eLib.NotificationService.Models
{
    public class PaginationResult<T>
    {
        public PaginationResult(IEnumerable<T> items, int count, int paginationParametersPageNumber, int paginationParametersPageSize)
        {
            Items = items;
            TotalCount = count;
            TotalPages = (int) Math.Ceiling(count / (double) paginationParametersPageSize);
            PageNumber = paginationParametersPageNumber;
            PageSize = paginationParametersPageSize;
        }

        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}