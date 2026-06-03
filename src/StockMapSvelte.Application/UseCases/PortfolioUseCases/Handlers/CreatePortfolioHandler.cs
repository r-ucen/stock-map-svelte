using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class CreatePortfolioHandler
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;

    public CreatePortfolioHandler(IUserContext userContext, IPortfolioRepository portfolioRepository, IUserSettingRepository userSettingRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
        _userSettingRepository = userSettingRepository;
    }

    public async Task<PortfolioStockDto> Handle(CreatePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cmd.PortfolioName))
        {
            throw new PortfolioNameMissingException();
        }
        
        var userId = await _userContext.GetCurrentUserIdAsync();
        
        var nameExists = await _portfolioRepository.PortfolioNameExistsAsync(userId, cmd.PortfolioName.Trim(), cancellationToken);
        if (nameExists)
        {
            throw new PortfolioNameAlreadyExistsException(cmd.PortfolioName);
        }

        var entity = new Domain.Entities.Portfolio
        {
            Id = Guid.NewGuid(),
            Name = cmd.PortfolioName.Trim(),
            UserId = userId
        };

        var result = await _portfolioRepository.CreatePortfolioAsync(entity, cmd.TickerSymbols, cancellationToken);
        
        var portfolioCount = await _portfolioRepository.GetPortfolioCountByUserIdAsync(userId);
        
        if (portfolioCount == 1)
        {
            var setPortfolioAsDefaultResult = await _userSettingRepository.SetPortfolioAsDefaultAsync(userId, entity.Id);
            
            if (setPortfolioAsDefaultResult <= 0)
            {
                throw new PortfolioCreationFailedException("Failed to set portfolio as default.");
            }
        }
        
        if (result <= 0)
        {
            throw new PortfolioCreationFailedException("Failed to create portfolio.");
        }

        return new PortfolioStockDto()
        {
            PortfolioId = entity.Id,
            UserId = entity.UserId,
            PortfolioName = entity.Name ?? "",
            TickerSymbols = cmd.TickerSymbols ?? []
        };
    }
}