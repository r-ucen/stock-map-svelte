using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class EditPortfolioHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public EditPortfolioHandler(
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PortfolioStockDto> Handle(EditPortfolioCommand cmd, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var existing = await _unitOfWork.Portfolios.GetByIdAsync(cmd.PortfolioId, cancellationToken);
        if (existing == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
            
        existing.ValidateOwnership(currentUserId);
        
        if (!existing.HasChanges(cmd.PortfolioName, cmd.TickerSymbols))
        {
            throw new PortfolioUnchangedException("Portfolio name and stocks are unchanged.");
        }
        
        var nameExists = await _unitOfWork.Portfolios.NameExistsAsync(currentUserId, cmd.PortfolioId, cmd.PortfolioName, cancellationToken);
        if (nameExists) { throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName); }
        
        var upperTickerSymbols = cmd.TickerSymbols
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();
        
        var uninitializedStocks = await _unitOfWork.Stocks.
            GetUninitializedTickerSymbolsAsync(upperTickerSymbols, cancellationToken);
        
        if (uninitializedStocks.Count != 0) { throw new StocksNotInitializedException(uninitializedStocks); }
        
        var stocks = await _unitOfWork.Stocks.FindTrackedAsync(s => upperTickerSymbols.Contains(s.TickerSymbol), cancellationToken);

        existing.UpdateStocks(stocks);
        existing.UpdateName(cmd.PortfolioName);
        
        _unitOfWork.Portfolios.Update(existing);

        var result = await _unitOfWork.CommitAsync(cancellationToken);
        if (result <= 0) { throw new PortfolioEditFailedException("Failed to edit portfolio."); }
        
        return new PortfolioStockDto
        {
            PortfolioId = existing.Id,
            UserId = existing.UserId,
            PortfolioName = cmd.PortfolioName.Trim(),
            TickerSymbols = cmd.TickerSymbols
        };
    }
}