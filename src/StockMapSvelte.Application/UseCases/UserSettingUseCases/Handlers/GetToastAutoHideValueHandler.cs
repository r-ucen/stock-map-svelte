using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class GetToastAutoHideValueHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;
    
    public GetToastAutoHideValueHandler(IUserSettingRepository userSettingRepository, IUserContext userContext)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
    }
    
    public async Task<bool> Handle()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        return await _userSettingRepository.GetToastAutoHideValueAsync(currentUserId) ?? true;
    }
}