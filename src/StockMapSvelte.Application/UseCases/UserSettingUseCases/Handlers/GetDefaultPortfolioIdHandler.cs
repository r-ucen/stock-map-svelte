using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class GetDefaultPortfolioIdHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;
    
    public GetDefaultPortfolioIdHandler(IUserSettingRepository userSettingRepository, IUserContext userContext)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
    }
    
    public async Task<Guid> Handle()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        return await _userSettingRepository.GetDefaultPortfolioIdAsync(currentUserId) ?? Guid.Empty;
    }
}