using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;

namespace StockMapSvelte.Infrastructure.Repositories;

public class UserSettingRepository : IUserSettingRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UserSettingRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> SetPortfolioAsDefaultAsync(string userId, Guid portfolioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = new UserSetting
            {
                UserId = userId,
                DefaultPortfolioId = portfolioId
            };

            context.UserSettings.Add(userSetting);
        }
        else
        {
            userSetting.DefaultPortfolioId = portfolioId;
        }

        return await context.SaveChangesAsync();
    }
    
    public async Task<Guid?> GetDefaultPortfolioIdAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var userSetting = await context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);

        return userSetting?.DefaultPortfolioId;
    }

    public async Task<int> SetToastAutoHideValueAsync(string userId, bool autoHide)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = new UserSetting
            {
                UserId = userId,
                ToastAutoHide = autoHide
            };

            context.UserSettings.Add(userSetting);
        }
        else
        {
            userSetting.ToastAutoHide = autoHide;
        }

        return await context.SaveChangesAsync();
    }

    public async Task<bool?> GetToastAutoHideValueAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var userSetting = await context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);
        
        return userSetting?.ToastAutoHide;
    }

    public async Task<int?> GetToastAutoHideDelayMs(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var userSetting = await context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);

        return userSetting?.ToastAutoHideDelayMs;
    }
    
    public async Task<int> SetToastAutoHideDelayMsAsync(string userId, int delayMs)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = new UserSetting
            {
                UserId = userId,
                ToastAutoHideDelayMs = delayMs
            };

            context.UserSettings.Add(userSetting);
        }
        else
        {
            userSetting.ToastAutoHideDelayMs = delayMs;
        }

        return await context.SaveChangesAsync();
    }
}