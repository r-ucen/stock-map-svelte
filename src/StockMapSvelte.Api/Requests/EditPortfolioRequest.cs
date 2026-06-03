namespace StockMapSvelte.Api.Requests;

public record EditPortfolioRequest(string PortfolioName, IList<string> TickerSymbols);