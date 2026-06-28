using System.ComponentModel.DataAnnotations.Schema;
using StockMapSvelte.Domain.Exceptions.Portfolio;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(Portfolio))]
public class Portfolio : Entity<Guid>
{
    public required string UserId { get; set; }
    public required string Name { get; set; }

    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public static Portfolio Create(string userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new PortfolioNameMissingException();
        }
        
        return new Portfolio()
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            UserId = userId
        };
    }
    
    public void AssignStocks(IEnumerable<Stock> stocks)
    {
        Stocks = stocks.ToList();
    }
    
    public void ValidateOwnership(string userId)
    {
        if (UserId != userId)
        {
            throw new UnauthorizedAccessException($"You do not have permission to modify portfolio with id: '{Id}'.");
        }
    }
}