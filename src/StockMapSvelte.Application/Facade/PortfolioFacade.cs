using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
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
    
    public async Task<PortfolioStockDto> CreatePortfolioAsync(CreatePortfolioCommand cmd)
    {
        return await _createPortfolioHandler.Handle(cmd);
    }

    public async Task<IReadOnlyList<PortfolioStockDto>> GetAllPortfolioStockViewModelsAsync()
    {
        return await _getAllPortfoliosHandler.Handle();
    }

    public async Task<IReadOnlyList<PortfolioStockDto>> GetPortfoliosByUserIdAsync()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var query = new GetPortfoliosByUserIdQuery
        {
            UserId = currentUserId
        };
        
        return await _getPortfoliosByUserIdHandler.Handle(query);
    }

    public async Task EditPortfolioAsync(EditPortfolioCommand cmd)
    {
        await _editPortfolioHandler.Handle(cmd);
    }

    public async Task DeletePortfolioAsync(Guid portfolioId)
    {
        var cmd = new DeletePortfolioCommand
        {
            PortfolioId = portfolioId
        };

        await _deletePortfolioHandler.Handle(cmd);
    }

    public async Task<PortfolioStockDto> GetPortfolioStockByIdAsync(GetPortfolioByIdQuery query)
    {
        return await _getPortfolioByIdHandler.Handle(query);
    }
}