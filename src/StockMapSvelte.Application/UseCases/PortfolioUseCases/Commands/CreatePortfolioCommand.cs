namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

public class CreatePortfolioCommand
{
    public string PortfolioName { get; set; } = "";
    public IList<string> TickerSymbols { get; set; } = new List<string>();
}