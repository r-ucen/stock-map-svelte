using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IUserFacade
{
    Task DeleteUserAsync(string userId);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserAsync(string id);
}