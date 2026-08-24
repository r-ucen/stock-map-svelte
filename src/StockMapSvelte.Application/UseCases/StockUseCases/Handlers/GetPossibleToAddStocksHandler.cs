using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetPossibleToAddStocksHandler
{
    private readonly IUnitOfWork  _unitOfWork;
    
    public GetPossibleToAddStocksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IReadOnlyList<StockDto>> Handle(GetPossibleToAddStocksQuery query, CancellationToken cancellationToken)
    {
        var filter = query.Filter.ToUpper();
        
        var upperTickersInPortfolio = query.StocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();
        
        return await _unitOfWork.Stocks.GetPossibleToAddAsync(filter, upperTickersInPortfolio, cancellationToken);
    }
}