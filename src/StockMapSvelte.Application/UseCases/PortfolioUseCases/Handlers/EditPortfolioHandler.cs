using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
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
    
    public async Task<PortfolioStockDto> Handle(EditPortfolioCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.PortfolioName))
        {
            throw new PortfolioNameMissingException();
        }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var existing = await _portfolioRepository.GetPortfolioByIdAsync(cmd.PortfolioId);
        if (existing == null)
        {
            throw new PortfolioNotFoundException("Portfolio not found.");
        }
        
        if (cmd.PortfolioName == existing.Name && cmd.TickerSymbols.SequenceEqual(existing.Stocks.Select(s => s.TickerSymbol)))
        {
            throw new PortfolioUnchangedException("Portfolio name and stocks are unchanged.");
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
        
        var portfolioDto = new PortfolioStockDto
        {
            PortfolioId = result.Id,
            UserId = result.UserId,
            PortfolioName = result.Name ?? "",
            TickerSymbols = result.Stocks.Select(s => s.TickerSymbol).ToList() ?? []
        };
        
        return portfolioDto;
    }
}