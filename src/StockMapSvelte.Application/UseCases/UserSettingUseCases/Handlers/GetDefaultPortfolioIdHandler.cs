using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.UserSetting;

namespace StockMapSvelte.Application.UseCases.UserSettingUseCases.Handlers;

public class GetDefaultPortfolioIdHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public GetDefaultPortfolioIdHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }
    
    public async Task<Guid> Handle(CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var defaultPortfolioId = await _unitOfWork.UserSettings.GetDefaultPortfolioIdAsync(currentUserId, cancellationToken);
        return defaultPortfolioId ?? throw new GetDefaultPortfolioIdFailedException();
    }
}