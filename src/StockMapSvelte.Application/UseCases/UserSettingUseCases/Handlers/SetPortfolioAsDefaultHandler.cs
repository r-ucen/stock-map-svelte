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
    
    public async Task<bool> Handle(SetPortfolioAsDefaultCommand cmd)
    {
        var existing = await _portfolioRepository.GetPortfolioByIdAsync(cmd.PortfolioId);
        if (existing == null)
        {
            throw new PortfolioNotFoundException("Portfolio not found.");
        }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        if (existing.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException($"User {currentUserId} does not have permission to set this portfolio as default.");
        }
        
        var result = await _userSettingRepository.SetPortfolioAsDefaultAsync(currentUserId, cmd.PortfolioId);
        
        return result > 0;
    }
}