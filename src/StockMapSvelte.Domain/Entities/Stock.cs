using System.ComponentModel.DataAnnotations.Schema;
using StockMapSvelte.Domain.Exceptions.Stock;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(Stock))]
public class Stock : Entity<Guid>
{
    public required string TickerSymbol { get; set; }
    public bool IsInitialized { get; set; }

    // Navigation properties
    public StockProfile StockProfile { get; set; } = null!;
    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

    public static Stock Create(string tickerSymbol)
    {
        if (string.IsNullOrWhiteSpace(tickerSymbol))
        {
            throw new MissingTickerSymbolException("Ticker symbol field is required.");
        }

        return new Stock
        {
            Id = Guid.NewGuid(),
            TickerSymbol = tickerSymbol.Trim().ToUpperInvariant()
        };
    }
    
    public void Update(string newTickerSymbol)
    {
        if (string.IsNullOrWhiteSpace(newTickerSymbol))
        {
            throw new MissingTickerSymbolException("Ticker symbol field is required.");
        }

        var normalizedTicker = newTickerSymbol.Trim().ToUpperInvariant();

        TickerSymbol = normalizedTicker;
        IsInitialized = false; 
    }
}