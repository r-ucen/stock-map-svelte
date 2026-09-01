using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetAllPortfoliosHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetAllPortfoliosHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PagedResponse<PortfolioDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Portfolios.GetAllAsync(filter, cancellationToken);
    }
}