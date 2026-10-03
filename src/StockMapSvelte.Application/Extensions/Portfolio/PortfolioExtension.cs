using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Extensions.Portfolio;

public static class PortfolioExtension
{
    extension(Domain.Entities.Portfolio p)
    {
        public PortfolioDto ToPortfolioDto()
        {
            return new PortfolioDto()
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
            };
        }
    }
}