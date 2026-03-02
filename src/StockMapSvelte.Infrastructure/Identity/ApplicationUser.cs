using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Domain.Entities.Interfaces;

namespace StockMapSvelte.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<string>, IUser<string>
{
    public UserSetting? UserSetting { get; set; }
}