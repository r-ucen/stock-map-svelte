using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken);
}