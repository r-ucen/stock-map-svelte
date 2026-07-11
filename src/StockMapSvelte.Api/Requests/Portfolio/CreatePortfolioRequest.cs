namespace StockMapSvelte.Api.Requests.Portfolio;

public record CreatePortfolioRequest(string PortfolioName, IList<string> TickerSymbols);