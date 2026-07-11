namespace StockMapSvelte.Api.Requests.Portfolio;

public record EditPortfolioRequest(string PortfolioName, IList<string> TickerSymbols);