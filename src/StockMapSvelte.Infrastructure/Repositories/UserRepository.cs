using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.User;

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

    public async Task<PagedResponse<UserDto>> GetAllAsyncQueried(QueryFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);
        
        var query = _userManager.Users.AsNoTracking().AsQueryable();
        
        query = query.ApplySearch(filter.Search, filter.SearchBy);
        
        var totalRecords = await query.CountAsync(cancellationToken);
        
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Email" : filter.SortBy);
        
        var usersList = await query
            .ApplyPagination(pageNumber, pageSize)
            .ToListAsync(cancellationToken);
        
        var users = new List<UserDto>();
        
        foreach (var u in usersList)
        {
            var roles = await _userManager.GetRolesAsync(u);
    
            users.Add(new UserDto(
                u.Id, 
                u.Email ?? string.Empty,  
                u.UserName ?? string.Empty, 
                roles
            ));
        }
        
        return new PagedResponse<UserDto>
        {
            Data = users,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public async Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null) { return new UserDto(string.Empty, string.Empty, string.Empty, []); }

        return new UserDto
        (
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            await _userManager.GetRolesAsync(user)
        );
    }
}