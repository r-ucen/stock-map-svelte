using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;

namespace StockMapSvelte.Infrastructure.Repositories;

public class UserSettingRepository(ApplicationDbContext dbContext) : IUserSettingRepository
{
    public async Task<UserSetting?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken: cancellationToken);
    }

    public async Task<UserSetting> AddAsync(UserSetting entity, CancellationToken cancellationToken = default)
    {
        await dbContext.UserSettings.AddAsync(entity, cancellationToken);
        return entity;
    }
    
    public async Task<Guid?> GetDefaultPortfolioIdAsync(string userId, CancellationToken cancellationToken)
    {
        var userSetting = await dbContext.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);

        return userSetting?.DefaultPortfolioId;
    }
}