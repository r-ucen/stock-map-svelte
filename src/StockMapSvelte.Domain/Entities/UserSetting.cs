using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(UserSetting))]
public class UserSetting
{
    [Key]
    public string UserId { get; set; } = null!;

    public Guid? DefaultPortfolioId { get; set; }

    // Toast settings
    public bool ToastAutoHide { get; set; } = true;
    public int ToastAutoHideDelayMs { get; set; } = 3000;
}