namespace StockMapSvelte.Application.DTOs;

public record StockDto
{
    public Guid Id { get; set; }
    public string TickerSymbol { get; set; } = string.Empty;
}