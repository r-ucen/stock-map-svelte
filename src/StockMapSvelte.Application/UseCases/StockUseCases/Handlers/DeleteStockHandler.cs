using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class DeleteStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IUserContext _userContext;
    
    public DeleteStockHandler(
        IStockRepository stockRepository,
        IUserContext userContext)
    {
        _stockRepository = stockRepository;
        _userContext = userContext;
    }
    
    public async Task Handle(DeleteStockCommand cmd)
    {
        if (!await _userContext.IsInRoleAsync("Admin") && !await _userContext.IsInRoleAsync("Manager"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to delete stocks.");
        }
        
        var exists = await _stockRepository.StockExistsAsync(cmd.StockId);
        if (!exists)
        {
            throw new StockNotFoundException($"Stock not found. Probably already deleted");
        }
        
        var result = await _stockRepository.DeleteStockAsync(cmd.StockId);
        
        if (result <= 0)
        {
            throw new StockDeletionFailedException("Failed to delete stock.");
        }
    }
}