using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Stock;

public class StocksNotInDatabaseException: AppException
{
    public StocksNotInDatabaseException(IReadOnlyList<string> uninitializedStocksTickerSymbols) : base($"The following stocks are not in the database: {string.Join(", ", uninitializedStocksTickerSymbols)}",
        400)
    { }
}