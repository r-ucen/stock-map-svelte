using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class PortfolioFacade : IPortfolioFacade
{
    private readonly IUserContext _userContext;
    
    private readonly CreatePortfolioHandler _createPortfolioHandler;
    private readonly DeletePortfolioHandler _deletePortfolioHandler;
    private readonly EditPortfolioHandler _editPortfolioHandler;
    private readonly GetPortfoliosByUserIdHandler _getPortfoliosByUserIdHandler;
    private readonly GetAllPortfoliosHandler _getAllPortfoliosHandler;
    private readonly GetPortfolioByIdHandler _getPortfolioByIdHandler;
    

    public PortfolioFacade(
        IUserContext userContext,
        CreatePortfolioHandler createPortfolioHandler,
        DeletePortfolioHandler deletePortfolioHandler,
        EditPortfolioHandler editPortfolioHandler,
        GetPortfoliosByUserIdHandler getPortfoliosByUserIdHandler,
        GetAllPortfoliosHandler getAllPortfoliosHandler,
        GetPortfolioByIdHandler getPortfolioByIdHandler
        )
    {
        _userContext = userContext;
        _createPortfolioHandler = createPortfolioHandler;
        _deletePortfolioHandler = deletePortfolioHandler;
        _editPortfolioHandler = editPortfolioHandler;
        _getPortfoliosByUserIdHandler = getPortfoliosByUserIdHandler;
        _getAllPortfoliosHandler = getAllPortfoliosHandler;
        _getPortfolioByIdHandler = getPortfolioByIdHandler;
    }
    
    public async Task<PortfolioStockDto> CreatePortfolioAsync(CreatePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        return await _createPortfolioHandler.Handle(cmd, cancellationToken);
    }

    public async Task<PagedResponse<PortfolioStockDto>> GetAllPortfolioStockViewModelsQueriedAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _getAllPortfoliosHandler.Handle(filter, cancellationToken);
    }

    public async Task<IReadOnlyList<PortfolioStockDto>> GetPortfoliosByUserIdAsync(CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var query = new GetPortfoliosByUserIdQuery(currentUserId);
        
        return await _getPortfoliosByUserIdHandler.Handle(query, cancellationToken);
    }

    public async Task<PortfolioStockDto> EditPortfolioAsync(EditPortfolioCommand cmd, CancellationToken cancellationToken)
    {
        return await _editPortfolioHandler.Handle(cmd, cancellationToken);
    }

    public async Task DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        var cmd = new DeletePortfolioCommand(portfolioId);

        await _deletePortfolioHandler.Handle(cmd, cancellationToken);
    }

    public async Task<PortfolioStockDto> GetPortfolioStockByIdAsync(GetPortfolioByIdQuery query, CancellationToken cancellationToken)
    {
        return await _getPortfolioByIdHandler.Handle(query, cancellationToken);
    }
}