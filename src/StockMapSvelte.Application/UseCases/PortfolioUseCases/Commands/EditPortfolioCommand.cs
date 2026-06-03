namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

public record EditPortfolioCommand
{
    public Guid PortfolioId { get; set; }
    public string PortfolioName { get; set; } = null!;
    public IList<string> TickerSymbols { get; set; } = [];
}