using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.Exceptions.UserSetting;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class GetPortfoliosByUserIdHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserSettingRepository _userSettingRepository;
    
    public GetPortfoliosByUserIdHandler(IPortfolioRepository portfolioRepository, IUserSettingRepository userSettingRepository)
    {
        _portfolioRepository = portfolioRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task<IReadOnlyList<PortfolioDto>> Handle(GetPortfoliosByUserIdQuery query, CancellationToken cancellationToken)
    {
        var portfolios = await _portfolioRepository.GetPortfoliosByUserIdAsync(query.UserId, cancellationToken);
        var defaultPortfolioId = await _userSettingRepository.GetDefaultPortfolioIdAsync(query.UserId, cancellationToken);

        if (portfolios == null)
        {
            return new List<PortfolioDto>();
        }

        if (portfolios.Count == 0)
        {
            var defaultPortfolio = Portfolio.Create(query.UserId, "Default portfolio");
            var createResult = await _portfolioRepository.CreatePortfolioAsync(defaultPortfolio, [], cancellationToken);
            if (createResult <= 0)
            {
                throw new PortfolioCreationFailedException("Failed to create default portfolio.");
            }
            
            portfolios = [defaultPortfolio];

            var setDefaultResult = await _userSettingRepository.SetPortfolioAsDefaultAsync(query.UserId, defaultPortfolio.Id);
            if (setDefaultResult <= 0)
            {
                throw new FailedToSetDefaultPortfolioException("Failed to set default portfolio.");
            }
            defaultPortfolioId = defaultPortfolio.Id;
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