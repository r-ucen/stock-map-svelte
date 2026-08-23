using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;

public class DeletePortfolioHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public DeletePortfolioHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(DeletePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var existing = await _unitOfWork.Portfolios.GetByIdAsync(cmd.PortfolioId, cancellationToken);
        if (existing == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
        
        existing.ValidateOwnership(currentUserId);
        
        _unitOfWork.Portfolios.Remove(existing);
        
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        if (result <= 0) { throw new PortfolioDeletionFailedException("Failed to delete portfolio."); }
    }
}