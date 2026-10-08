using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Services;

public class StockProfileUpdateTracker : IStockProfileUpdateTracker
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.MinValue;
}