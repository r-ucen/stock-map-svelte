using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Infrastructure.Database.Seeding;

internal class StockInit
{
    public IList<Stock> GetStocks()
    {
        IList<Stock> stocks = new List<Stock>()
        {
            new Stock
            {
                Id = new Guid("9a480000-0389-c018-9dbd-08de40aae72b"),
                TickerSymbol = "AAPL"
            },
            new Stock
            {
                Id = new Guid("9a480000-0389-c018-a702-08de40aae72b"),
                TickerSymbol = "MSFT"
            },
            new Stock
            {
                Id = new Guid("9a480000-0389-c018-a70b-08de40aae72b"),
                TickerSymbol = "GOOGL"
            },
            new Stock
            {
                Id = new Guid("9a480000-0389-c018-a70e-08de40aae72b"),
                TickerSymbol = "AVGO"
            },
            new Stock
            {
                Id = new Guid("9a480000-0389-c018-a712-08de40aae72b"),
                TickerSymbol = "RKLB"
            },
            new Stock
            {
                Id = new Guid("9a480000-0389-c018-a7c2-08de40aae72b"),
                TickerSymbol = "TSLA"
            }
        };
        return stocks;
    }
}