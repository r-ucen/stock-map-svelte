namespace StockMapSvelte.Application.Abstractions;

public interface IStockUpdateService
{
    Task UpdateAsync(CancellationToken cancellationToken);
}