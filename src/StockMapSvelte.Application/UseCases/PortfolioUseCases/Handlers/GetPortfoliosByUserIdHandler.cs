using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetPortfoliosByUserIdHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    
    public GetPortfoliosByUserIdHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public async Task<IReadOnlyList<PortfolioStockDto>> Handle(GetPortfoliosByUserIdQuery query)
    {
        return await _portfolioRepository.GetPortfoliosByUserIdAsync(query.UserId!) ?? new List<PortfolioStockDto>();
    }
    
}