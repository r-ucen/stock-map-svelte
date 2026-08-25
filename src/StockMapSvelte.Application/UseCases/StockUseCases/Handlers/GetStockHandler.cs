using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Stock;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetStockHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StockDto> Handle(GetStockQuery query, CancellationToken cancellationToken)
    {
        var stock = await _unitOfWork.Stocks.GetByIdAsync(query.StockId, cancellationToken);
        if (stock == null) { throw new StockNotFoundException($"Stock with id '{query.StockId}' was not found."); }

        return new StockDto(stock.Id, stock.TickerSymbol);
    }
}