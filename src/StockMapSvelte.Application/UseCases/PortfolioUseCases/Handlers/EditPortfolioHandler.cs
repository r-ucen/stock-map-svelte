using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class EditPortfolioHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IUserContext _userContext;
    
    public EditPortfolioHandler(
        IUserContext userContext,
        IPortfolioRepository portfolioRepository,
        IStockRepository stockRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
        _stockRepository = stockRepository;
    }
    
    public async Task<PortfolioStockDto> Handle(EditPortfolioCommand cmd, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var existing = await _portfolioRepository.GetPortfolioByIdAsync(cmd.PortfolioId, cancellationToken);
        if (existing == null)
        {
            throw new PortfolioNotFoundException("Portfolio not found.");
        }
            
        existing.ValidateOwnership(currentUserId);
        
        if (!existing.HasChanges(cmd.PortfolioName, cmd.TickerSymbols))
        {
            throw new PortfolioUnchangedException("Portfolio name and stocks are unchanged.");
        }
        
        var nameExists = await _portfolioRepository.PortfolioNameExistsAsync(currentUserId, cmd.PortfolioId, cmd.PortfolioName, cancellationToken);
        if (nameExists)
        {
            throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName);
        }
        
        var uninitializedStocks = await _stockRepository.GetUninitializedStocks(cmd.TickerSymbols, cancellationToken);
        if (uninitializedStocks.Count != 0)
        {
            throw new StocksNotInitializedException(uninitializedStocks);
        }

        var result = await _portfolioRepository.EditPortfolioAsync(cmd.PortfolioId, cmd.PortfolioName, cmd.TickerSymbols, cancellationToken);
        if (result <= 0)
        {
            throw new PortfolioEditFailedException("Failed to edit portfolio.");
        }
        
        return new PortfolioStockDto
        {
            PortfolioId = existing.Id,
            UserId = existing.UserId,
            PortfolioName = cmd.PortfolioName.Trim(),
            TickerSymbols = cmd.TickerSymbols
        };
    }
}