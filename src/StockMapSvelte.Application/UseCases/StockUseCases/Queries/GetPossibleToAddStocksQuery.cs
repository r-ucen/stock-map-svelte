namespace StockMapSvelte.Application.UseCases.StockUseCases.Queries;

public record GetPossibleToAddStocksQuery(string Filter, IList<string> StocksInPortfolio);