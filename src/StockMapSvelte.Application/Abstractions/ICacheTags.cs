namespace StockMapSvelte.Application.Abstractions;

public interface ICacheTags
{
    IPortfolioTags Portfolio { get; }
    
    public interface IPortfolioTags
    {
        string TreemapData();
    }
}