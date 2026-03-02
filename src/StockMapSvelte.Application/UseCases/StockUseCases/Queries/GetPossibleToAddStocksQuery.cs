namespace StockMapSvelte.Application.UseCases.StockUseCases.Queries;

public class GetPossibleToAddStocksQuery
{
    public string Filter { get; set; } = string.Empty;
    public IList<string> StocksInPortfolio { get; set; } = new List<string>();
    public CancellationToken CancellationToken { get; set; }
}