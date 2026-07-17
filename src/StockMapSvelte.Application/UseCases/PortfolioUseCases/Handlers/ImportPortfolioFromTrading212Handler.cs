using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class ImportPortfolioFromTrading212Handler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;
    private readonly ITrading212Client _trading212Client;
    private readonly IStockClient _stockClient;

    public ImportPortfolioFromTrading212Handler(
        IUserContext userContext,
        IPortfolioRepository portfolioRepository,
        IUserSettingRepository userSettingRepository,
        IStockRepository stockRepository,
        ITrading212Client trading212Client,
        IStockClient stockClient)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
        _userSettingRepository = userSettingRepository;
        _stockRepository = stockRepository;
        _trading212Client = trading212Client;
        _stockClient = stockClient;
    }
    
    public async Task<PortfolioStockDto> Handle(ImportPortfolioFromTrading212Command cmd, CancellationToken cancellationToken)
    {
        var userId = await _userContext.GetCurrentUserIdAsync();
        
        var portfolioCount = await _portfolioRepository.GetPortfolioCountByUserIdAsync(userId, cancellationToken);
        if (portfolioCount >= 5) { throw new MaxPortfoliosReachedException(5); }
        
        var portfolio = Portfolio.Create(userId, cmd.PortfolioName);
        
        var nameExists = await _portfolioRepository.PortfolioNameExistsAsync(userId, portfolio.Name, cancellationToken);
        if (nameExists) { throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName); }

        var yahooTickerSymbols = new List<string>();

        var positions = await _trading212Client.GetPositionsAsync(cmd.Trading212ApiKey, cmd.Trading212ApiSecret,
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
        
        var missingStocks = await _stockRepository.GetMissingStocksAsync(upperTickerSymbols, cancellationToken);
        var uninitializedStocks = await _stockRepository.GetUninitializedStocks(upperTickerSymbols, cancellationToken);
        
        var invalidStocks = missingStocks.Union(uninitializedStocks).ToList();
        var validTickerSymbols = upperTickerSymbols.Except(invalidStocks).ToList();
        
        if (validTickerSymbols.Count == 0) { throw new PortfolioCreationFailedException(
            "None of the stocks in your Trading212 portfolio are initialized or exist in our database."); }

        var result = await _portfolioRepository.CreatePortfolioAsync(portfolio, validTickerSymbols, cancellationToken);
        if (result <= 0) { throw new PortfolioCreationFailedException("Failed to create portfolio."); }
        
        if (portfolioCount == 1)
        {
            var setPortfolioAsDefaultResult = await _userSettingRepository.SetPortfolioAsDefaultAsync(userId, portfolio.Id);
            
            if (setPortfolioAsDefaultResult <= 0)
            {
                throw new PortfolioCreationFailedException("Failed to set portfolio as default.");
            }
        }

        return new PortfolioStockDto()
        {
            PortfolioId = portfolio.Id,
            UserId = portfolio.UserId,
            PortfolioName = portfolio.Name,
            TickerSymbols = validTickerSymbols
        };
    }
}