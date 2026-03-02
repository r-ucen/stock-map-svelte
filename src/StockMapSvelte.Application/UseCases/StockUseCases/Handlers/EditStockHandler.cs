using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class EditStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockProfileRepository _stockProfileRepository;
    private readonly IUserContext _userContext;
    private readonly IStockClient _stockClient;
    
    public EditStockHandler(
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
    
    public async Task Handle(EditStockCommand cmd)
    {
        if (!await _userContext.IsInRoleAsync("Admin") && !await _userContext.IsInRoleAsync("Manager"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to edit stocks.");
        }
        
        if (await _stockRepository.StockExistsAsync(cmd.TickerSymbol))
        {
            throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{cmd.TickerSymbol}' already exists.");
        }
        
        if (!await _stockClient.TickerExists(cmd.TickerSymbol))
        {
            throw new InvalidTickerSymbolException($"This ticker symbol: '{cmd.TickerSymbol}' is not valid.");
        }
        
        if (!await _stockRepository.StockExistsAsync(cmd.Id))
        {
            throw new StockNotFoundException($"Stock with id: '{cmd.Id}' was not found.", cmd.Id.ToString());
        }
        
        var editStockResult = await _stockRepository.EditStockAsync(cmd.Id, cmd.TickerSymbol.Trim().ToUpperInvariant());
        
        if (editStockResult <= 0)
        {
            throw new Exception("Failed to edit the stock.");
        }
        
        var stockProfiles = await _stockClient.GetStockProfilesAsync();

        var saveStockProfilesResult = await _stockProfileRepository.SaveStockProfilesAsync(stockProfiles);
        if (saveStockProfilesResult <= 0)
        {
            throw new CreateStockFailException("Failed to edit the stock.");
        }
    }
}