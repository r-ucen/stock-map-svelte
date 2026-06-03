namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

public record CreatePortfolioCommand(string PortfolioName, IList<string> TickerSymbols);