using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class DeleteStockHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteStockHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(DeleteStockCommand cmd, CancellationToken cancellationToken)
    {
        var stock = await _unitOfWork.Stocks.GetByIdAsync(cmd.StockId, cancellationToken);
        if (stock is null) { throw new StockNotFoundException($"Stock with id '{cmd.StockId}' was not found"); }
        
        _unitOfWork.Stocks.Remove(stock);
        
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        if (result <= 0) { throw new StockDeletionFailedException("Failed to delete stock."); }
    }
}