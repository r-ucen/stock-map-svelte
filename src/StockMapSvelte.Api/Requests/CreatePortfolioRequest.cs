namespace StockMapSvelte.Api.Requests;

public record CreatePortfolioRequest(string PortfolioName, IList<string> TickerSymbols);