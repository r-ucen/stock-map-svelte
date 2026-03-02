using System.ComponentModel.DataAnnotations.Schema;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(Stock))]
public class Stock : Entity<Guid>
{
    public required string TickerSymbol { get; set; }

    // Navigation properties
    public StockProfile StockProfile { get; set; } = null!;
    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
}