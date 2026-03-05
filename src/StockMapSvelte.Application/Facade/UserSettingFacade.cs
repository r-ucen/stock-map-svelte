using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

namespace StockMapSvelte.Application.Facade;

public class UserSettingFacade : IUserSettingFacade
{
    
    private readonly SetPortfolioAsDefaultHandler _setPortfolioAsDefaultHandler;
    private readonly GetDefaultPortfolioIdHandler _getDefaultPortfolioIdHandler;
    private readonly GetToastAutoHideValueHandler _getToastAutoHideValueHandler;
    private readonly SetToastAutoHideValueHandler _setToastAutoHideValueHandler;
    private readonly GetToastAutoHideDelayMsHandler _getToastAutoHideDelayMs;
    private readonly SetToastAutoHideDelayMsHandler _setToastAutoHideDelayMs;
    
    public UserSettingFacade(
        SetPortfolioAsDefaultHandler setPortfolioAsDefaultHandler,
        GetDefaultPortfolioIdHandler getDefaultPortfolioIdHandler,
        GetToastAutoHideValueHandler getToastAutoHideValueHandler,
        SetToastAutoHideValueHandler setToastAutoHideValueHandler,
        GetToastAutoHideDelayMsHandler getToastAutoHideDelayMsHandler,
        SetToastAutoHideDelayMsHandler setToastAutoHideDelayMsHandler)
    {
        _setPortfolioAsDefaultHandler = setPortfolioAsDefaultHandler;
        _getDefaultPortfolioIdHandler = getDefaultPortfolioIdHandler;
        _getToastAutoHideValueHandler = getToastAutoHideValueHandler;
        _setToastAutoHideValueHandler = setToastAutoHideValueHandler;
        _getToastAutoHideDelayMs = getToastAutoHideDelayMsHandler;
        _setToastAutoHideDelayMs = setToastAutoHideDelayMsHandler;
    }
    
    public async Task SetPortfolioAsDefaultAsync(SetPortfolioAsDefaultCommand cmd)
    {
        await _setPortfolioAsDefaultHandler.Handle(cmd);
    }
    
    public async Task<Guid> GetDefaultPortfolioIdAsync()
    {
        return await _getDefaultPortfolioIdHandler.Handle();
    }
    
    public async Task SetToastAutoHideValueAsync(SetToastAutoHideValueCommand cmd)
    {
        await _setToastAutoHideValueHandler.Handle(cmd);
    }
    
    public async Task<bool> GetToastAutoHideValueAsync()
    {
        return await _getToastAutoHideValueHandler.Handle();
    }

    public async Task<int> GetToastAutoHideDelayMs()
    {
        return await _getToastAutoHideDelayMs.Handle();
    }

    public async Task SetToastAutoHideDelayMsAsync(SetToastAutoHideDelayMsCommand cmd)
    {
        await _setToastAutoHideDelayMs.Handle(cmd);
    }
}