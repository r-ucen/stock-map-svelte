using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class CreatePortfolioHandler
{
    private readonly IUserContext _userContext;
    
    private readonly IUnitOfWork _unitOfWork;

    public CreatePortfolioHandler(
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<PortfolioStockDto> Handle(CreatePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        var userId = await _userContext.GetCurrentUserIdAsync();
        
        var portfolioCount = await _unitOfWork.Portfolios.GetCountByUserIdAsync(userId, cancellationToken);
        if (portfolioCount >= 5) { throw new MaxPortfoliosReachedException(5); }
        
        var portfolio = Portfolio.Create(userId, cmd.PortfolioName);
        
        var nameExists = await _unitOfWork.Portfolios.NameExistsAsync(userId, cmd.PortfolioName, cancellationToken);
        if (nameExists) { throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName); }
        
        var upperTickerSymbols = cmd.TickerSymbols
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();
        
        var missingStocks = await _unitOfWork.Stocks.
            GetMissingTickerSymbolsAsync(upperTickerSymbols, cancellationToken);
        if (missingStocks.Count != 0) { throw new StocksNotInDatabaseException(missingStocks); }

        var uninitializedStocks = await _unitOfWork.Stocks.
            GetUninitializedTickerSymbolsAsync(upperTickerSymbols, cancellationToken);
        if (uninitializedStocks.Count != 0) { throw new StocksNotInitializedException(uninitializedStocks); }
        
        var stocksInPortfolio = await _unitOfWork.Stocks
            .FindTrackedAsync(s => upperTickerSymbols.Contains(s.TickerSymbol), cancellationToken);
        
        portfolio.AssignStocks(stocksInPortfolio);
        
        await _unitOfWork.Portfolios.AddAsync(portfolio, cancellationToken);
        
        if (portfolioCount == 0)
        {
            var userSetting = await _unitOfWork.UserSettings.GetAsync(userId, cancellationToken);

            if (userSetting == null)
            {
                userSetting = UserSetting.CreateForUser(userId);
                await _unitOfWork.UserSettings.AddAsync(userSetting, cancellationToken);
            }
            
            userSetting.SetDefaultPortfolio(portfolio.Id);
        }

        var result = await _unitOfWork.CommitAsync(cancellationToken);
        if (result <= 0) { throw new PortfolioCreationFailedException("Failed to create portfolio."); }
        
        return new PortfolioStockDto()
        {
            PortfolioId = portfolio.Id,
            UserId = portfolio.UserId,
            PortfolioName = portfolio.Name,
            TickerSymbols = cmd.TickerSymbols ?? []
        };
    }
}