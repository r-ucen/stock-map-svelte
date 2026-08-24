using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class ImportPortfolioFromTrading212Handler
{
    private readonly IUserContext _userContext;
    private readonly ITrading212Client _trading212Client;
    private readonly IStockClient _stockClient;
    private readonly IUnitOfWork _unitOfWork;

    public ImportPortfolioFromTrading212Handler(
        IUserContext userContext,
        ITrading212Client trading212Client,
        IStockClient stockClient,
        IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _trading212Client = trading212Client;
        _stockClient = stockClient;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PortfolioDto> Handle(ImportPortfolioFromTrading212Command cmd, CancellationToken cancellationToken)
    {
        var userId = await _userContext.GetCurrentUserIdAsync();
        
        var portfolioCount = await _unitOfWork.Portfolios.GetCountForUserAsync(userId, cancellationToken);
        if (portfolioCount >= 5) { throw new MaxPortfoliosReachedException(5); }
        
        var portfolio = Portfolio.Create(userId, cmd.PortfolioName);
        
        var nameExists = await _unitOfWork.Portfolios.NameExistsAsync(userId, portfolio.Name, cancellationToken);
        if (nameExists) { throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName); }

        var yahooTickerSymbols = new List<string>();

        var positions = await _trading212Client.GetPositionsAsync(
            cmd.Trading212ApiKey,
            cmd.Trading212ApiSecret,
            cmd.IsDemoTrading212Account);
        if (positions == null) { throw new PortfolioCreationFailedException("No positions found in Trading212 account."); }
        
        foreach (var position in positions)
        {
            var yahooSymbol = await _stockClient.GetYahooTickerSymbol(position.Instrument.Isin);
            if (!string.IsNullOrWhiteSpace(yahooSymbol)) { yahooTickerSymbols.Add(yahooSymbol); }
        }
        
        var upperTickerSymbols = yahooTickerSymbols
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();
        
        var missingTickerSymbols =
            await _unitOfWork.Stocks.GetMissingTickerSymbolsAsync(upperTickerSymbols, cancellationToken);
        var uninitializedTickerSymbols = 
            await _unitOfWork.Stocks.GetUninitializedTickerSymbolsAsync(upperTickerSymbols, cancellationToken);
        
        var invalidTickerSymbols = missingTickerSymbols.Union(uninitializedTickerSymbols).ToList();
        var validTickerSymbols = upperTickerSymbols.Except(invalidTickerSymbols).ToList();
        
        if (validTickerSymbols.Count == 0) { throw new PortfolioCreationFailedException(
            "None of the stocks in your Trading212 portfolio are initialized or exist in our database."); }
        
        var stocksInPortfolio = await _unitOfWork.Stocks
            .FindTrackedAsync(s => validTickerSymbols.Contains(s.TickerSymbol), cancellationToken);
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
        
        return new PortfolioDto()
        {
            PortfolioId = portfolio.Id,
            UserId = portfolio.UserId,
            PortfolioName = portfolio.Name,
            TickerSymbols = validTickerSymbols
        };
    }
}