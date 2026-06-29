using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.UserSetting;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;
using StockMapSvelte.Domain.Entities;

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

    public async Task Handle(SetToastAutoHideDelayMsCommand cmd)
    {
        UserSetting.ValidateDelayRange(cmd.DelayMs);
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var result = await _userSettingRepository.SetToastAutoHideDelayMsAsync(currentUserId, cmd.DelayMs);
        if (result < 0) { throw new Exception("Failed to set toast auto hide delay ms"); }
    }
}