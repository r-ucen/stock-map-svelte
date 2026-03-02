using System.ComponentModel.DataAnnotations.Schema;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(Portfolio))]
public class Portfolio : Entity<Guid>
{
    public required string UserId { get; set; }
    public string? Name { get; set; }

    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}