using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class SetToastAutoHideValueHandler
{
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IUserContext _userContext;
    
    public SetToastAutoHideValueHandler(IUserSettingRepository userSettingRepository, IUserContext userContext)
    {
        _userSettingRepository = userSettingRepository;
        _userContext = userContext;
    }
    
    public async Task<bool> Handle(SetToastAutoHideValueCommand autoHide)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var result = await _userSettingRepository.SetToastAutoHideValueAsync(currentUserId, autoHide.Value);
        
        return result > 0;
    }
}