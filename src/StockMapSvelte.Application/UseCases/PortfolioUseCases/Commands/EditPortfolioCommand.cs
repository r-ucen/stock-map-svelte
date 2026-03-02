namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

public class EditPortfolioCommand
{
    public Guid PortfolioId { get; set; }
    public string PortfolioName { get; set; } = null!;
    public IList<string> TickerSymbols { get; set; } = [];
}