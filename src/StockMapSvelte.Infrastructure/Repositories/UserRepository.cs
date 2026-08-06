using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.User;

namespace StockMapSvelte.Infrastructure.Repositories;

public class UserRepository(UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken)
    {
        if (userId == null) { throw new ArgumentNullException(userId); }
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) { throw new InvalidOperationException($"The user with id: {userId} was not found"); }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<PagedResponse<UserDto>> GetAllAsyncQueried(QueryFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);
        
        var query = userManager.Users.AsNoTracking().AsQueryable();
        
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
            var roles = await userManager.GetRolesAsync(u);
    
            users.Add(new UserDto(
                u.Id, 
                u.Email ?? string.Empty,  
                u.UserName ?? string.Empty, 
                roles,
                await userManager.IsLockedOutAsync(u)
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

    public async Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null) { return null; }

        return new UserDto
        (
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            await userManager.GetRolesAsync(user),
            await userManager.IsLockedOutAsync(user)
        );
    }
}