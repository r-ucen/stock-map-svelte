namespace StockMapSvelte.Application.Exceptions.Stock;

public class StocksNotInitializedException: AppException
{
    public StocksNotInitializedException(IList<string> uninitializedStocksTickerSymbols) : base($"The following stocks are not initialized: {string.Join(", ", uninitializedStocksTickerSymbols)}",
        400)
    { }
}