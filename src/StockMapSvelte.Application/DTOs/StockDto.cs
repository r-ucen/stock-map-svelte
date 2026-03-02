namespace StockMapSvelte.Application.DTOs;

public class StockDto
{
    public Guid Id { get; set; }
    public string TickerSymbol { get; set; } = string.Empty;
}