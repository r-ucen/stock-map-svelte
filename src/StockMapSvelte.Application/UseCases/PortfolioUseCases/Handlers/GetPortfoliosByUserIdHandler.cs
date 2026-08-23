using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetPortfoliosByUserIdHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetPortfoliosByUserIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PortfolioDto>> Handle(GetPortfoliosByUserIdQuery query, CancellationToken cancellationToken)
    {
        var portfolios = await _unitOfWork.Portfolios.GetForUserAsync(query.UserId, cancellationToken);
        var defaultPortfolioId = await _unitOfWork.UserSettings.GetDefaultPortfolioIdAsync(query.UserId, cancellationToken);

        if (portfolios.Count == 0)
        {
            var defaultPortfolio = Portfolio.Create(query.UserId, "Default portfolio");
            await _unitOfWork.Portfolios.AddAsync(defaultPortfolio, cancellationToken);
            
            portfolios = [defaultPortfolio];
            
            var userSetting = await _unitOfWork.UserSettings.GetAsync(query.UserId, cancellationToken);
            if (userSetting == null)
            {
                userSetting = UserSetting.CreateForUser(query.UserId);
                await _unitOfWork.UserSettings.AddAsync(userSetting, cancellationToken);
            }
            
            userSetting.SetDefaultPortfolio(defaultPortfolio.Id);

            defaultPortfolioId = defaultPortfolio.Id;
            
            var result = await _unitOfWork.CommitAsync(cancellationToken);
            if (result <= 0) { throw new PortfolioCreationFailedException("Failed to create default portfolio."); }
        }
        
        return portfolios
            .Select(p => new PortfolioDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name,
                IsDefault = p.Id == defaultPortfolioId,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
            }).ToList();
    }
}