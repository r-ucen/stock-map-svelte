using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetPortfolioByIdHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public GetPortfolioByIdHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PortfolioDto> Handle(GetPortfolioByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var portfolio = await _unitOfWork.Portfolios.GetByIdAsync(currentUserId, query.PortfolioId, cancellationToken);
        if (portfolio == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
        
        var defaultPortfolioId = await _unitOfWork.UserSettings.GetDefaultPortfolioIdAsync(currentUserId, cancellationToken);
        
        return new PortfolioDto()
        {
            PortfolioId = portfolio.Id,
            UserId = portfolio.UserId,
            PortfolioName = portfolio.Name,
            IsDefault = portfolio.Id == defaultPortfolioId,
            TickerSymbols = portfolio.Stocks.Select(s => s.TickerSymbol).ToList()
        };
    }
}