namespace StockMapSvelte.Application.DTOs.Common;

public class QueryFilter
{
    public int PageNumber { get; set; } = 1;
    public string? SearchBy { get; set; }
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? Search { get; set; }
}