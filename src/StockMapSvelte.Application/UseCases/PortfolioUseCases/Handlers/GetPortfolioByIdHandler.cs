using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetPortfolioByIdHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public GetPortfolioByIdHandler(IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
        _userContext = userContext;
    }
    
    public async Task<PortfolioStockDto> Handle(GetPortfolioByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var portfolio = await _portfolioRepository.GetPortfolioByIdForUserAsync(currentUserId, query.PortfolioId, cancellationToken);
        
        return portfolio ?? throw new PortfolioNotFoundException("Portfolio not found.");
    }
}