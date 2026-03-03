namespace StockMapSvelte.Api.Requests;

public class EditPortfolioRequest
{
    public string PortfolioName { get; set; } = string.Empty;
    public IList<string> TickerSymbols { get; set; } = new List<string>();
}