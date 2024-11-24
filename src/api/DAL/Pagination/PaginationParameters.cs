namespace eLib.DAL.Pagination;

public class PaginationParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SearchFraze { get; set; }
}