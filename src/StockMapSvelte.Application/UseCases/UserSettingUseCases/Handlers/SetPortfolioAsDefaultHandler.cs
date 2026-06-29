using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class SetPortfolioAsDefaultHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public SetPortfolioAsDefaultHandler(IUserSettingRepository userSettingRepository, IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task Handle(SetPortfolioAsDefaultCommand cmd, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var existing = await _portfolioRepository.GetPortfolioByIdAsync(cmd.PortfolioId, cancellationToken);
        if (existing == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
        
        existing.ValidateOwnership(currentUserId);
        
        var result = await _userSettingRepository.SetPortfolioAsDefaultAsync(currentUserId, cmd.PortfolioId);

        if (result < 0) { throw new Exception("Failed to set portfolio as default."); }
    }
}