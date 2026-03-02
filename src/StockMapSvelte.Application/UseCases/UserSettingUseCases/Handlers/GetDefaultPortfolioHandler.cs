using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class GetDefaultPortfolioHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUserContext _userContext;
    
    public GetDefaultPortfolioHandler(IUserSettingRepository userSettingRepository, IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task<Guid> Handle()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        return await _userSettingRepository.GetDefaultPortfolioAsync(currentUserId) ?? Guid.Empty;
    }
}