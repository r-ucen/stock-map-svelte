using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetAllUsersQueriedHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueriedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PagedResponse<UserDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Users.GetAllAsyncQueried(filter, cancellationToken);
    }
}