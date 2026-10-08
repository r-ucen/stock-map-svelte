namespace StockMapSvelte.Application.Abstractions;

public interface IStockProfileUpdateTracker
{
    DateTimeOffset LastUpdated { get; set; }
}