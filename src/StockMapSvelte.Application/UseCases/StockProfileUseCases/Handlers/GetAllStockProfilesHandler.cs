using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.StockProfileUseCases.Handlers;

public class GetAllStockProfilesHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetAllStockProfilesHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PagedResponse<StockStockProfileDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _unitOfWork.StockProfiles.GetAllStockProfilesAsync(filter, cancellationToken);
    }
}