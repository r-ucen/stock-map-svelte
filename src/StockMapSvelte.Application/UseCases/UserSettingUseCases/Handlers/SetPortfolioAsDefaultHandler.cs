using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class SetPortfolioAsDefaultHandler
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    
    public SetPortfolioAsDefaultHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(SetPortfolioAsDefaultCommand cmd, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var existing = await _unitOfWork.Portfolios.GetByIdAsync(currentUserId, cmd.PortfolioId, cancellationToken);
        if (existing == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
        
        existing.ValidateOwnership(currentUserId);

        var userSetting = await _unitOfWork.UserSettings.GetAsync(currentUserId, cancellationToken);
        if (userSetting == null)
        {
            userSetting = UserSetting.CreateForUser(currentUserId);
            await _unitOfWork.UserSettings.AddAsync(userSetting, cancellationToken);
        }
        
        userSetting.SetDefaultPortfolio(existing.Id);
        
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        if (result < 0) { throw new Exception("Failed to set portfolio as default."); }
    }
}