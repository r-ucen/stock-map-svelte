using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetAllPortfoliosHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    
    public GetAllPortfoliosHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task<PagedResponse<PortfolioStockDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _portfolioRepository.GetAllPortfolioStockViewModelsAsync(filter, cancellationToken);
    }
}