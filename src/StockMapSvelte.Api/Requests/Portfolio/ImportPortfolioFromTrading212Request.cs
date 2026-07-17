namespace StockMapSvelte.Api.Requests.Portfolio;

public record ImportPortfolioFromTrading212Request(string Trading212ApiKey, string Trading212ApiSecret, bool IsDemoTrading212Account, string PortfolioName);