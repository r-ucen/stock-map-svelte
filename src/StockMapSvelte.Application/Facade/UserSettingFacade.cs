using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

namespace StockMapSvelte.Application.Facade;

public class UserSettingFacade : IUserSettingFacade
{
    
    private readonly SetPortfolioAsDefaultHandler _setPortfolioAsDefaultHandler;
    private readonly GetDefaultPortfolioIdHandler _getDefaultPortfolioIdHandler;
    
    public UserSettingFacade(
        SetPortfolioAsDefaultHandler setPortfolioAsDefaultHandler,
        GetDefaultPortfolioIdHandler getDefaultPortfolioIdHandler)
    {
        _setPortfolioAsDefaultHandler = setPortfolioAsDefaultHandler;
        _getDefaultPortfolioIdHandler = getDefaultPortfolioIdHandler;
    }
    
    public async Task SetPortfolioAsDefaultAsync(SetPortfolioAsDefaultCommand cmd, CancellationToken cancellationToken)
    {
        await _setPortfolioAsDefaultHandler.Handle(cmd, cancellationToken);
    }
    
    public async Task<Guid> GetDefaultPortfolioIdAsync(CancellationToken cancellationToken)
    {
        return await _getDefaultPortfolioIdHandler.Handle(cancellationToken);
    }
}