using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Domain.Exceptions.Portfolio;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class CreatePortfolioHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;

    public CreatePortfolioHandler(
        IUserContext userContext,
        IPortfolioRepository portfolioRepository,
        IUserSettingRepository userSettingRepository,
        IStockRepository stockRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
        _userSettingRepository = userSettingRepository;
        _stockRepository = stockRepository;
    }

    public async Task<PortfolioStockDto> Handle(CreatePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        var userId = await _userContext.GetCurrentUserIdAsync();
        var portfolio = Portfolio.Create(userId, cmd.PortfolioName);
        
        var nameExists = await _portfolioRepository.PortfolioNameExistsAsync(userId, portfolio.Name, cancellationToken);
        if (nameExists)
        {
            throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName);
        }
        
        var upperTickerSymbols = cmd.TickerSymbols
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();

        var uninitializedStocks =
            await _stockRepository.GetUninitializedStocks(upperTickerSymbols, cancellationToken);
        
        if (uninitializedStocks.Count != 0)
        {
            throw new StocksNotInitializedException(uninitializedStocks);
        }

        var result = await _portfolioRepository.CreatePortfolioAsync(portfolio, cmd.TickerSymbols, cancellationToken);
        if (result <= 0)
        {
            throw new PortfolioCreationFailedException("Failed to create portfolio.");
        }
        
        var portfolioCount = await _portfolioRepository.GetPortfolioCountByUserIdAsync(userId);
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
            TickerSymbols = cmd.TickerSymbols ?? []
        };
    }
}