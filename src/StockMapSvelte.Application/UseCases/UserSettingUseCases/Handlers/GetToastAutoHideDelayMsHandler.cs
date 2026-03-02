using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class GetToastAutoHideDelayMsHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;
    
    public GetToastAutoHideDelayMsHandler(IUserSettingRepository userSettingRepository, IUserContext userContext)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
    }
    
    public async Task<int> Handle()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        return await _userSettingRepository.GetToastAutoHideDelayMs(currentUserId) ?? 3000;
    }
}