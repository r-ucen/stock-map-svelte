using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

namespace StockMapSvelte.Application.Facade;

public class UserSettingFacade : IUserSettingFacade
{
    private readonly IUserContext _userContext;
    
    private readonly SetPortfolioAsDefaultHandler _setPortfolioAsDefaultHandler;
    private readonly GetDefaultPortfolioHandler _getDefaultPortfolioHandler;
    private readonly GetToastAutoHideValueHandler _getToastAutoHideValueHandler;
    private readonly SetToastAutoHideValueHandler _setToastAutoHideValueHandler;
    private readonly GetToastAutoHideDelayMsHandler _getToastAutoHideDelayMs;
    private readonly SetToastAutoHideDelayMsHandler _setToastAutoHideDelayMs;
    
    public UserSettingFacade(
        IUserContext userContext,
        SetPortfolioAsDefaultHandler setPortfolioAsDefaultHandler,
        GetDefaultPortfolioHandler getDefaultPortfolioHandler,
        GetToastAutoHideValueHandler getToastAutoHideValueHandler,
        SetToastAutoHideValueHandler setToastAutoHideValueHandler,
        GetToastAutoHideDelayMsHandler getToastAutoHideDelayMsHandler,
        SetToastAutoHideDelayMsHandler setToastAutoHideDelayMsHandler)
    {
        _userContext = userContext;
        _setPortfolioAsDefaultHandler = setPortfolioAsDefaultHandler;
        _getDefaultPortfolioHandler = getDefaultPortfolioHandler;
        _getToastAutoHideValueHandler = getToastAutoHideValueHandler;
        _setToastAutoHideValueHandler = setToastAutoHideValueHandler;
        _getToastAutoHideDelayMs = getToastAutoHideDelayMsHandler;
        _setToastAutoHideDelayMs = setToastAutoHideDelayMsHandler;
    }
    
    public async Task<bool> SetPortfolioAsDefaultAsync(Guid portfolioId)
    {
        var cmd = new UseCases.UserSettingUseCases.Commands.SetPortfolioAsDefaultCommand
        {
            PortfolioId = portfolioId
        };
        
        return await _setPortfolioAsDefaultHandler.Handle(cmd);
    }
    
    public async Task<Guid> GetDefaultPortfolioAsync()
    {
        return await _getDefaultPortfolioHandler.Handle();
    }
    
    public async Task<bool> SetToastAutoHideValueAsync(bool value)
    {
        var cmd = new SetToastAutoHideValueCommand
        {
            Value = value
        };
        
        return await _setToastAutoHideValueHandler.Handle(cmd);
    }
    
    public async Task<bool> GetToastAutoHideValueAsync()
    {
        return await _getToastAutoHideValueHandler.Handle();
    }

    public async Task<int> GetToastAutoHideDelayMs()
    {
        return await _getToastAutoHideDelayMs.Handle();
    }

    public async Task<bool> SetToastAutoHideDelayMsAsync(int value)
    {
        var cmd = new SetToastAutoHideDelayMsCommand
        {
            DelayMs = value
        };
        
        return await _setToastAutoHideDelayMs.Handle(cmd);
    }
}