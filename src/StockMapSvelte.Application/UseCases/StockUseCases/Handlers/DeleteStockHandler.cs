using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class DeleteStockHandler
{
    private readonly IStockRepository _stockRepository;
    
    public DeleteStockHandler(
        IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }
    
    public async Task Handle(DeleteStockCommand cmd, CancellationToken cancellationToken)
    {
        var exists = await _stockRepository.StockExistsAsync(cmd.StockId, cancellationToken);
        if (!exists)
        {
            throw new StockNotFoundException($"Stock with id '{cmd.StockId}' was not found");
        }
        
        var result = await _stockRepository.DeleteStockAsync(cmd.StockId, cancellationToken);
        if (result <= 0)
        {
            throw new StockDeletionFailedException("Failed to delete stock.");
        }
    }
}