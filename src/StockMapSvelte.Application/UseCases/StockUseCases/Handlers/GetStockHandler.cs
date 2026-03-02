using StockMapSvelte.Application.UseCases.StockUseCases.Queries;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IUserContext _userContext;
    
    public GetStockHandler(IStockRepository stockRepository, IUserContext userContext)
    {
        _stockRepository = stockRepository;
        _userContext = userContext;
    }

    public async Task<StockDto> Handle(GetStockQuery query)
    {
        if (!await _userContext.IsInRoleAsync("Admin") && !await _userContext.IsInRoleAsync("Manager"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to get StockViewModel by id.");
        }
        
        return await _stockRepository.GetStockViewModelByIdAsync(query.StockId) ?? new StockDto();
    }
}