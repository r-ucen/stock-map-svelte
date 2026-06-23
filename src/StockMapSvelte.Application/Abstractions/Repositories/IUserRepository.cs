using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<PagedResponse<UserDto>> GetAllAsyncQueried(QueryFilter filter, CancellationToken cancellationToken);
    Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken);
}