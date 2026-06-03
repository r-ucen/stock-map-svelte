using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetAllPortfoliosHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    
    public GetAllPortfoliosHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task<IReadOnlyList<PortfolioStockDto>> Handle(CancellationToken cancellationToken)
    {
        return await _portfolioRepository.GetAllPortfolioStockViewModelsAsync(cancellationToken) ?? new List<PortfolioStockDto>();
    }
}