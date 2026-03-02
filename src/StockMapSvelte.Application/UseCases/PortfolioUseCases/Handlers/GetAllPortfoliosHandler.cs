using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetAllPortfoliosHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public GetAllPortfoliosHandler(IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task<IReadOnlyList<PortfolioStockDto>> Handle()
    {
        if (!await _userContext.IsInRoleAsync("Admin") && !await _userContext.IsInRoleAsync("Manager"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to access all portfolios.");
        }
        
        return await _portfolioRepository.GetAllPortfolioStockViewModelsAsync() ?? new List<PortfolioStockDto>();
    }
}