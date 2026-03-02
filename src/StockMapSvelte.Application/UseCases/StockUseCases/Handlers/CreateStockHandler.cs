using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockProfileRepository _stockProfileRepository;
    private readonly IUserContext _userContext;
    private readonly IStockClient _stockClient;
    
    public CreateStockHandler(
        IStockRepository stockRepository,
        IStockProfileRepository stockProfileRepository,
        IUserContext userContext,
        IStockClient stockClient)
    {
        _stockRepository = stockRepository;
        _stockProfileRepository = stockProfileRepository;
        _userContext = userContext;
        _stockClient = stockClient;
    }

    public async Task Handle(CreateStockCommand cmd)
    {
        if (!await _userContext.IsInRoleAsync("Admin") && !await _userContext.IsInRoleAsync("Manager"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to create stocks.");
        }
        
        if (string.IsNullOrWhiteSpace(cmd.TickerSymbol))
        {
            throw new MissingTickerSymbolException("Ticker symbol field is required.");
        }
        
        var ticker = cmd.TickerSymbol.Trim().ToUpperInvariant();

        if (!await _stockClient.TickerExists(ticker))
        {
            throw new InvalidTickerSymbolException($"This ticker symbol: '{ticker}' is not valid.");
        }
        
        var exists = await _stockRepository.StockExistsAsync(ticker);
        if (exists)
        {
            throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{cmd.TickerSymbol}' already exists.");
        }

        var entity = new Stock
        {
            TickerSymbol = ticker,
            Id = Guid.NewGuid()
        };
        
        var createStockResult = await _stockRepository.CreateStockAsync(entity);
        if (createStockResult <= 0)
        {
            throw new CreateStockFailException("Failed to create the stock.");
        }
        
        var stockProfiles = await _stockClient.GetStockProfilesAsync();

        var saveStockProfilesResult = await _stockProfileRepository.SaveStockProfilesAsync(stockProfiles);
        if (saveStockProfilesResult <= 0)
        {
            throw new CreateStockFailException("Failed to create the stock.");
        }
    }
}