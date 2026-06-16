namespace StockMapSvelte.Application.DTOs;

public record StockCreationFailure(string Ticker, string Error);
public record CreateStocksResponse(string[] CreatedStocks, StockCreationFailure[] FailedToCreateStocks);