using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockMapSvelte.Domain.Exceptions.UserSetting;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(UserSetting))]
public class UserSetting
{
    public string UserId { get; set; } = null!;
    public Guid? DefaultPortfolioId { get; set; }

    // Toast settings
    public bool ToastAutoHide { get; set; } = true;
    public int ToastAutoHideDelayMs { get; set; } = 3000;
    
    public static UserSetting CreateForUser(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return new UserSetting { UserId = userId };
    }

    public void SetToastAutoHide(bool autoHide)
    {
        ToastAutoHide = autoHide;
    }

    public void SetToastAutoHideDelay(int delayMs)
    {
        if (delayMs < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMs), "Delay must be non-negative.");
        }
        
        ToastAutoHideDelayMs = delayMs;
    }

    public void SetDefaultPortfolio(Guid? portfolioId)
    {
        DefaultPortfolioId = portfolioId;
    }

    public static void ValidateDelayRange(int delayMs)
    {
        if (delayMs is <= 500 or > 60000)
        {
            throw new InvalidDelayException(delayMs);
        }
    }
}