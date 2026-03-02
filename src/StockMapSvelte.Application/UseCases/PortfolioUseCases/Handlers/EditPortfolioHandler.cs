using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class EditPortfolioHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public EditPortfolioHandler(IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task Handle(EditPortfolioCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.PortfolioName))
        {
            throw new PortfolioNameMissingException();
        }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var existing = await _portfolioRepository.GetPortfolioByIdAsync(cmd.PortfolioId);
        if (existing == null)
        {
            throw new Exception("Portfolio not found.");
        }
            
        if (existing.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException($"User {currentUserId} does not have permission to edit portfolio with id {cmd.PortfolioId}.");
        }
        
        var nameExists = await _portfolioRepository.PortfolioNameExistsAsync(currentUserId, cmd.PortfolioId, cmd.PortfolioName);
        if (nameExists)
        {
            throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName);
        }

        var result = await _portfolioRepository.EditPortfolioAsync(cmd.PortfolioId, cmd.PortfolioName.Trim(), cmd.TickerSymbols);
        
        if (result <= 0)
        {
            throw new Exception("Failed to update portfolio.");
        }
    }
}