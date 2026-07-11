using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;

namespace StockMapSvelte.Infrastructure.Repositories;

public class UserSettingRepository(ApplicationDbContext dbContext) : IUserSettingRepository
{
    public async Task<int> SetPortfolioAsDefaultAsync(string userId, Guid portfolioId)
    {
        var userSetting = await dbContext.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = UserSetting.CreateForUser(userId);
            await dbContext.UserSettings.AddAsync(userSetting);
        }
        
        userSetting.SetDefaultPortfolio(portfolioId);

        return await dbContext.SaveChangesAsync();
    }
    
    public async Task<Guid?> GetDefaultPortfolioIdAsync(string userId, CancellationToken cancellationToken)
    {
        var userSetting = await dbContext.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);

        return userSetting?.DefaultPortfolioId;
    }

    public async Task<int> SetToastAutoHideValueAsync(string userId, bool autoHide)
    {
        var userSetting = await dbContext.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = UserSetting.CreateForUser(userId);
            dbContext.UserSettings.Add(userSetting);
        }
        
        userSetting.SetToastAutoHide(autoHide);

        return await dbContext.SaveChangesAsync();
    }

    public async Task<bool?> GetToastAutoHideValueAsync(string userId)
    {
        var userSetting = await dbContext.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);
        
        return userSetting?.ToastAutoHide;
    }

    public async Task<int?> GetToastAutoHideDelayMs(string userId)
    {
        var userSetting = await dbContext.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);

        return userSetting?.ToastAutoHideDelayMs;
    }
    
    public async Task<int> SetToastAutoHideDelayMsAsync(string userId, int delayMs)
    {
        var userSetting = await dbContext.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = UserSetting.CreateForUser(userId);
            dbContext.UserSettings.Add(userSetting);
        }
        
        userSetting.SetToastAutoHideDelay(delayMs);

        return await dbContext.SaveChangesAsync();
    }
}