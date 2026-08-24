using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetAllStocksQueriedHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetAllStocksQueriedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PagedResponse<StockDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Stocks.GetAllAsync(filter, cancellationToken);
    }
}