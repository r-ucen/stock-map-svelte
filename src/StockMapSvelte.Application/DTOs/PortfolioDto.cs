namespace StockMapSvelte.Application.DTOs;

public record PortfolioDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsDefault { get; set; }
    public IList<string> TickerSymbols { get; set; } = [];
}