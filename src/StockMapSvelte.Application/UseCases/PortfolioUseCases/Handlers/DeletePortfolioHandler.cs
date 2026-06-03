using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class DeletePortfolioHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public DeletePortfolioHandler(IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task Handle(DeletePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var existing = await _portfolioRepository.GetPortfolioByIdAsync(cmd.PortfolioId, cancellationToken);
        if (existing == null)
        {
            throw new PortfolioNotFoundException("Portfolio not found.");
        }
            
        if (existing.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException($"User {currentUserId} does not have permission to delete portfolio with id {cmd.PortfolioId}.");
        }
        
        var result = await _portfolioRepository.DeletePortfolioAsync(existing.Id, cancellationToken);
        
        if (result <= 0)
        {
            throw new PortfolioDeletionFailedException("Failed to delete portfolio.");
        }
    }
}