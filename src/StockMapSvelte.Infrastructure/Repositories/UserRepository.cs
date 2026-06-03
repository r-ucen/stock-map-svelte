using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Infrastructure.Identity;

namespace StockMapSvelte.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken)
    {
        if (userId == null)
        {
            throw new ArgumentNullException(userId);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException($"The user with id: {userId} was not found");
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToArrayAsync(cancellationToken);
        var userViewModels = new List<UserDto>();

        foreach (var user in users)
        {
            userViewModels.Add(new UserDto
            (
                user.Id,
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                await _userManager.GetRolesAsync(user)
            ));
        }

        return userViewModels;
    }

    public async Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            return new UserDto(string.Empty, string.Empty, string.Empty, []);
        }

        var vm = new UserDto
        (
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            await _userManager.GetRolesAsync(user)
        );

        return vm;
    }
}