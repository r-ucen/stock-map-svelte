using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.TreeMapUseCases.Handlers;

public class GetTreemapDataHandler
{
    private readonly ITreeMapRepository _treeMapRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public GetTreemapDataHandler(
        ITreeMapRepository treeMapRepository,
        IPortfolioRepository portfolioRepository,
        IUserContext userContext)
    {
        _treeMapRepository = treeMapRepository;
        _portfolioRepository = portfolioRepository;
        _userContext = userContext;
    }
    
    public async Task<TreemapDataDto> Handle(Guid portfolioId, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(portfolioId, cancellationToken);
            
        if (portfolio == null)
        {
            throw new PortfolioNotFoundException("Portfolio not found.");
        }
            
        if (portfolio.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException($"You do not have permission to view portfolio with id {portfolioId}.");
        }
        
        return await _treeMapRepository.GetTreemapDataViewModelByIdAsync(portfolioId) ?? new TreemapDataDto();
    }
}