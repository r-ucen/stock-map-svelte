using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class SetToastAutoHideDelayMsHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;
    
    public SetToastAutoHideDelayMsHandler(IUserSettingRepository userSettingRepository, IUserContext userContext)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
    }

    public async Task<bool> Handle(SetToastAutoHideDelayMsCommand cmd)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var result = await _userSettingRepository.SetToastAutoHideDelayMsAsync(currentUserId, cmd.DelayMs);
        return result > 0;
    }
}