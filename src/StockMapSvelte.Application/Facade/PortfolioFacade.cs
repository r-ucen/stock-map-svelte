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
    private readonly CreatePortfolioHandler _createPortfolioHandler;
    private readonly DeletePortfolioHandler _deletePortfolioHandler;
    private readonly EditPortfolioHandler _editPortfolioHandler;
    private readonly GetPortfoliosByUserIdHandler _getPortfoliosByUserIdHandler;
    private readonly GetAllPortfoliosHandler _getAllPortfoliosHandler;
    private readonly GetPortfolioByIdHandler _getPortfolioByIdHandler;
    private readonly ImportPortfolioFromTrading212Handler _importPortfolioFromTrading212Handler;
    

    public PortfolioFacade(
        CreatePortfolioHandler createPortfolioHandler,
        DeletePortfolioHandler deletePortfolioHandler,
        EditPortfolioHandler editPortfolioHandler,
        GetPortfoliosByUserIdHandler getPortfoliosByUserIdHandler,
        GetAllPortfoliosHandler getAllPortfoliosHandler,
        GetPortfolioByIdHandler getPortfolioByIdHandler,
        ImportPortfolioFromTrading212Handler importPortfolioFromTrading212Handler
        )
    {
        _createPortfolioHandler = createPortfolioHandler;
        _deletePortfolioHandler = deletePortfolioHandler;
        _editPortfolioHandler = editPortfolioHandler;
        _getPortfoliosByUserIdHandler = getPortfoliosByUserIdHandler;
        _getAllPortfoliosHandler = getAllPortfoliosHandler;
        _getPortfolioByIdHandler = getPortfolioByIdHandler;
        _importPortfolioFromTrading212Handler = importPortfolioFromTrading212Handler;
    }
    
    public async Task<PortfolioDto> CreatePortfolioAsync(CreatePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        return await _createPortfolioHandler.Handle(cmd, cancellationToken);
    }
    
    public async Task<PortfolioDto> ImportPortfolioFromTrading212(ImportPortfolioFromTrading212Command cmd, CancellationToken cancellationToken)
    {
        return await _importPortfolioFromTrading212Handler.Handle(cmd, cancellationToken);
    }

    public async Task<PagedResponse<PortfolioDto>> GetAllPortfolioStockViewModelsQueriedAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _getAllPortfoliosHandler.Handle(filter, cancellationToken);
    }

    public async Task<IReadOnlyList<PortfolioDto>> GetPortfoliosByUserIdAsync(GetPortfoliosByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await _getPortfoliosByUserIdHandler.Handle(query, cancellationToken);
    }

    public async Task<PortfolioDto> EditPortfolioAsync(EditPortfolioCommand cmd, CancellationToken cancellationToken)
    {
        return await _editPortfolioHandler.Handle(cmd, cancellationToken);
    }

    public async Task DeletePortfolioAsync(DeletePortfolioCommand cmd, CancellationToken cancellationToken)
    {
        await _deletePortfolioHandler.Handle(cmd, cancellationToken);
    }

    public async Task<PortfolioDto> GetPortfolioStockByIdAsync(GetPortfolioByIdQuery query, CancellationToken cancellationToken)
    {
        return await _getPortfolioByIdHandler.Handle(query, cancellationToken);
    }
}