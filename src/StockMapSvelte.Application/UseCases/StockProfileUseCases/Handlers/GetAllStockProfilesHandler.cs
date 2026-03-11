using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.StockProfileUseCases.Handlers;

public class GetAllStockProfilesHandler
{
    private readonly IStockProfileRepository _stockProfileRepository;
    private readonly IUserContext _userContext;
    
    public GetAllStockProfilesHandler(
        IStockProfileRepository stockProfileRepository,
        IUserContext userContext)
    {
        _stockProfileRepository = stockProfileRepository;
        _userContext = userContext;
    }
    
    public async Task<IReadOnlyList<StockStockProfileDto>> Handle()
    {
        return await _stockProfileRepository.GetAllStockProfilesAsync();
    }
        
}