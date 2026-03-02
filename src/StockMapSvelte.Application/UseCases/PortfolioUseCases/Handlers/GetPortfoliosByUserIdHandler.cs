using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetPortfoliosByUserIdHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public GetPortfoliosByUserIdHandler(IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<IReadOnlyList<PortfolioStockDto>> Handle(GetPortfoliosByUserIdQuery query)
    {
        if (query.UserId == null)
        {
            throw new ArgumentNullException(nameof(query.UserId), "UserId cannot be null.");
        }
        
        var isUserAuthenticated = await _userContext.IsUserAuthenticatedAsync();
        
        if (!isUserAuthenticated)
        {
            throw new UnauthorizedAccessException("Unauthorized access. User is not authenticated.");
        }
        
        return await _portfolioRepository.GetPortfoliosByUserIdAsync(query.UserId) ?? new List<PortfolioStockDto>();
    }
    
}