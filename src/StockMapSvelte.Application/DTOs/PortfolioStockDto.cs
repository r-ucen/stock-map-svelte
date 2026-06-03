namespace StockMapSvelte.Application.DTOs;

public record PortfolioStockDto
{
    public Guid PortfolioId { get; set; }
    public string UserId { get; set; } = null!;
    public string PortfolioName { get; set; } = null!;
    public bool IsDefault { get; set; }
    public IList<string> TickerSymbols { get; set; } = [];
}